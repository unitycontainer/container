using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class SetupDiagnostic : SetupPipeline
    {
        #region Implementation

        protected override ResolveDelegate<BuilderContext> TransientLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var registration = builder.Registration;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
#if !NET40
                // Check call stack for cyclic references
                ValidateCompositionStack(context.Parent, context.Type, context.Name);
#endif
                // Execute pipeline
                try
                {
                    // In Sync mode just execute pipeline
                    if (!context.Async) return pipeline(ref context);

                    // Async mode
                    var parent = IntPtr.Zero;
                    var list = context.List;
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
#if !NET40
                    unsafe
                    {
                        var thisContext = this;
                        parent = new IntPtr(Unsafe.AsPointer(ref thisContext));
                    }
#endif
                    // Create and return a task that creates an object
                    return Task.Factory.StartNew(() =>
                    {
                        var c = new BuilderContext
                        {
                            List = list,
                            Type = type,
                            ContainerContext = unity,
                            Registration = registration,
                            Overrides = overrides,
                            DeclaringType = type,
                            Parent = parent,
                        };

                        // Build the type
                        return pipeline(ref c);
                    });
                }
                catch (Exception ex) when (ex is InvalidRegistrationException || ex is CircularDependencyException)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    if (null != context.DeclaringType) throw;

                    var message = CreateMessage(ex);
                    throw new ResolutionFailedException(context.Type, context.Name, message, ex);
                }
            };
        }

        protected override ResolveDelegate<BuilderContext> PerResolveLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var registration = builder.Registration;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
#if !NET40
                // Check call stack for cyclic references
                ValidateCompositionStack(context.Parent, context.Type, context.Name);
#endif
                // Execute pipeline
                try
                {
                    object value;

                    // Get it from context
                    var manager = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    if (null != manager)
                    {
                        value = manager.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value)
                        {
                            return context.Async
                                ? Task.FromResult(value)
                                : value;
                        }
                    }

                    // In Sync mode just execute pipeline
                    if (!context.Async)
                    {
                        // Compose down the chain
                        value = pipeline(ref context);
                        manager?.SetValue(value, context.ContainerContext.Lifetime);

                        return value;
                    }

                    // Async mode
                    var list = context.List;
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
                    IntPtr parent = IntPtr.Zero;
#if !NET40
                    unsafe
                    {
                        var thisContext = this;
                        parent = new IntPtr(Unsafe.AsPointer(ref thisContext));
                    }
#endif
                    // Create and return a task that creates an object
                    return Task.Factory.StartNew(() =>
                    {
                        var c = new BuilderContext
                        {
                            List = list,
                            Type = type,
                            ContainerContext = unity,
                            Registration = registration,
                            Overrides = overrides,
                            DeclaringType = type,
                            Parent = parent,
                        };

                        // Execute pipeline
                        value = pipeline(ref c);
                        manager?.SetValue(value, c.ContainerContext.Lifetime);

                        return value;
                    });
                }
                catch (Exception ex) when (ex is InvalidRegistrationException || ex is CircularDependencyException)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    if (null != context.DeclaringType) throw;

                    var message = CreateMessage(ex);
                    throw new ResolutionFailedException(context.Type, context.Name, message, ex);
                }
            };
        }

        protected override ResolveDelegate<BuilderContext> DefaultLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var lifetime = builder.Registration.LifetimeManager;
            var synchronized = lifetime as SynchronizedLifetimeManager;
            var registration = builder.Registration;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
#if !NET40
                // Check call stack for cyclic references
                ValidateCompositionStack(context.Parent, context.Type, context.Name);
#endif
                // Execute pipeline
                try
                {
                    // Return if holds value
                    var value = lifetime.GetValue(context.ContainerContext.Lifetime);

                    if (LifetimeManager.NoValue != value)
                        return context.Async ? Task.FromResult(value) : value;

                    // In Sync mode just execute pipeline
                    if (!context.Async)
                    {
                        value = pipeline(ref context);

                        // Set value
                        lifetime.SetValue(value, context.ContainerContext.Lifetime);
                        return value;
                    }

                    // Async mode
                    var list = context.List;
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
                    IntPtr parent = IntPtr.Zero;
#if !NET40
                    unsafe
                    {
                        var thisContext = this;
                        parent = new IntPtr(Unsafe.AsPointer(ref thisContext));
                    }
#endif
                    // Create and return a task that creates an object
                    var task = Task.Factory.StartNew(() =>
                    {
                        var c = new BuilderContext
                        {
                            List = list,
                            Type = type,
                            ContainerContext = unity,
                            Registration = registration,
                            Overrides = overrides,
                            DeclaringType = type,
                            Parent = parent,
                        };

                        // Execute pipeline
                        var result = pipeline(ref c);

                        lifetime.SetValue(result, c.ContainerContext.Lifetime);

                        return result;
                    });

                    return task;
                }
                catch (Exception ex)
                {
                    synchronized?.Recover();

                    if (null == context.DeclaringType && (ex is InvalidRegistrationException || ex is CircularDependencyException))
                    {
                        var message = CreateMessage(ex);
                        throw new ResolutionFailedException(context.Type, context.Name, message, ex);
                    }
                    else throw;
                }
            };
        }

        #endregion


        #region Validation

        #if !NET40
        [SecuritySafeCritical]
        private void ValidateCompositionStack(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return;

            unsafe
            {
                var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                if (type == parentRef.Type && name == parentRef.Name)
                    throw new CircularDependencyException(parentRef.Type, parentRef.Name);

                ValidateCompositionStack(parentRef.Parent, type, name);
            }
        }
        #endif

        #endregion


        #region Error Message

        private static string CreateMessage(Exception ex, string? message = null)
        {
            const string line = "_____________________________________________________";

            var builder = new StringBuilder();
            builder.AppendLine(ex.Message);
            if (null == message)
            {
                builder.AppendLine(line);
                builder.AppendLine("Exception occurred while:");
            }

            foreach (DictionaryEntry item in ex.Data)
                builder.AppendLine(DataToString(item.Value));

            if (null != message)
            {
                builder.AppendLine(line);
                builder.AppendLine(message);
            }

            return builder.ToString();
        }

        private static string DataToString(object value)
        {
            switch (value)
            {
                case ParameterInfo parameter:
                    return $"    for parameter:  {parameter.Name}";

                case ConstructorInfo constructor:
                    var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"   on constructor:  {constructor.DeclaringType.Name}({ctorSignature})";

                case MethodInfo method:
                    var methodSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"        on method:  {method.Name}({methodSignature})";

                case PropertyInfo property:
                    return $"    for property:   {property.Name}";

                case FieldInfo field:
                    return $"       for field:   {field.Name}";

                case Type type:
                    return $"\n• while resolving:  {type.Name}";

                case Tuple<Type, string?> tuple:
                    return $"\n• while resolving:  {tuple.Item1.Name} registered with name: {tuple.Item2}";

                case Tuple<Type, Type> tuple:
                    return $"        mapped to:  {tuple.Item1?.Name}";
            }

            return value.ToString();
        }

        #endregion
    }
}
