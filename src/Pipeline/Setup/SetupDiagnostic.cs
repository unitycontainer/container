using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Pipeline
{
    public class SetupDiagnostic : SetupBuilder
    {
        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            // Pipeline
            var type = builder.Type;
            var name = builder.Name;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                // Setup Root Context
                if (null == context.DeclaringType)
                {
                    context.List = new PolicyList();
                    if (null != context.Overrides && 0 == context.Overrides.Length) context.Overrides = null;
                }
#if !NET40
                // Check call stack for cyclic references
                var value = GetPerResolveValue(context.Parent, context.Type, context.Name);
                if (LifetimeManager.NoValue != value) return value;
#endif
                try
                {
                    // Build the type
                    return pipeline(ref context);
                }
                catch (Exception ex) when (null == context.DeclaringType)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    var message = CreateMessage(ex);
                    throw new ResolutionFailedException(context.Type, context.Name, message, ex);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    throw;
                }
            };
        }

#if !NET40
        [SecuritySafeCritical]
        object GetPerResolveValue(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return LifetimeManager.NoValue;

            unsafe
            {
                var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                if (type != parentRef.Type || name != parentRef.Name)
                    return GetPerResolveValue(parentRef.Parent, type, name);

                var lifetimeManager = (LifetimeManager?)parentRef.Get(typeof(LifetimeManager));
                var result = null == lifetimeManager ? LifetimeManager.NoValue : lifetimeManager.GetValue();
                if (LifetimeManager.NoValue != result) return result;

                throw new InvalidOperationException($"Circular reference for Type: {parentRef.Type}, Name: {parentRef.Name}",
                        new CircularDependencyException());
            }
        }
#endif


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
