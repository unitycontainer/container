using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class ConstructorDiagnostic : ConstructorPipeline
    {
        #region Fields

        const string CannotConstructAbstractClass = "The current type, {0}, is an abstract class and cannot be constructed. Are you missing a type mapping?";
        const string CannotConstructDelegate = "The current type, {0}, is delegate and cannot be constructed. Unity only supports resolving Func&lt;T&gt; and Func&lt;IEnumerable&lt;T&gt;&gt; by default.";
        const string CannotConstructInterface = "The current type, {0}, is an interface and cannot be constructed. Are you missing a type mapping?";
        const string TypeIsNotConstructable = "The type {0} cannot be constructed. You must configure the container to supply this value.";

        private static readonly Expression[] CannotConstructInterfaceExpr = new [] {
            Expression.IfThen(NullEqualExisting,
                 Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructInterface),
                            PipelineContextExpression.Type))))};

        private static readonly Expression[] CannotConstructAbstractClassExpr = new [] {
            Expression.IfThen(NullEqualExisting,
                 Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructAbstractClass),
                            PipelineContextExpression.Type))))};

        private static readonly Expression[] CannotConstructDelegateExpr = new [] {
            Expression.IfThen(NullEqualExisting,
                 Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructDelegate),
                            PipelineContextExpression.Type))))};

        private static readonly Expression[] TypeIsNotConstructableExpr = new [] {
            Expression.IfThen(NullEqualExisting,
                 Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(TypeIsNotConstructable),
                            PipelineContextExpression.Type))))};

        #endregion


        #region Constructors

        public ConstructorDiagnostic(UnityContainer container) 
            : base(container)
        {
            container.Defaults.Set(typeof(Func<Type, InjectionMember, ConstructorInfo>), InjectionValidatingSelector);
        }

        #endregion


        #region Selection

        public override IEnumerable<object> Select(Type type, InjectionMember[]? injectionMembers)
        {
            var members = new List<InjectionMember>();

            // Select Injected Members
            foreach (var injectionMember in injectionMembers ?? EmptyCollection)
            {
                if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
                {
                    members.Add(injectionMember);
                }
            }

            switch (members.Count)
            {
                case 1:
                    return members.ToArray();

                case 0:
                    break;

                default:
                    return new[] { new InvalidRegistrationException($"Multiple Injection Constructors are registered for Type {type.FullName}") };
            }

            // Enumerate to array
            var constructors = DeclaredMembers(type).ToArray();
            if (1 >= constructors.Length)
                return constructors;

            var selection = new HashSet<ConstructorInfo>();

            // Select Attributed constructors
            foreach (var constructor in constructors)
            {
                foreach(var attribute in Markers)
                {
#if NET40
                    if (!constructor.IsDefined(attribute, true))
#else
                    if (!constructor.IsDefined(attribute))
#endif
                        continue;

                    selection.Add(constructor);
                }
            }

            switch (selection.Count)
            {
                case 1:
                    return selection.ToArray();

                case 0:
                    break;

                default:
                    return new[] { new InvalidRegistrationException($"Multiple Constructors are annotated for injection on Type {type.FullName}") };
            }

            // Select default
            return new[] { SelectMethod(type, constructors) };
        }

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        public override object? LegacySelector(Type type, ConstructorInfo[] members)
        {
            // TODO: Add validation to legacy selector
            Array.Sort(members, (x, y) => y?.GetParameters().Length ?? 0 - x?.GetParameters().Length ?? 0);

            switch (members.Length)
            {
                case 0:
                    return null;

                case 1:
                    return members[0];

                default:
                    var paramLength = members[0].GetParameters().Length;
                    if (members[1].GetParameters().Length == paramLength)
                    {
                        return new InvalidRegistrationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The type {0} has multiple constructors of length {1}. Unable to disambiguate.",
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return members[0];
            }
        }

        protected override object? SmartSelector(Type type, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors, (a, b) =>
            {
                var qtd = b.GetParameters().Length.CompareTo(a.GetParameters().Length);

                if (qtd == 0)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    return b.GetParameters().Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0)
                        .CompareTo(a.GetParameters().Sum(p => p.ParameterType.GetTypeInfo().IsInterface ? 1 : 0));
#else
                    return b.GetParameters().Sum(p => p.ParameterType.IsInterface ? 1 : 0)
                        .CompareTo(a.GetParameters().Sum(p => p.ParameterType.IsInterface ? 1 : 0));
#endif
                }
                return qtd;
            });

            int parametersCount = 0;
            ConstructorInfo? bestCtor = null;

            foreach (var ctorInfo in constructors)
            {
                var parameters = ctorInfo.GetParameters();

                if (null != bestCtor && parametersCount > parameters.Length) return bestCtor;
                parametersCount = parameters.Length;

#if NET40
                if (parameters.All(p => (null != p.DefaultValue && !(p.DefaultValue is DBNull)) || CanResolve(p)))
#else
                if (parameters.All(p => p.HasDefaultValue || CanResolve(p)))
#endif
                {
                    if (bestCtor == null)
                    {
                        bestCtor = ctorInfo;
                    }
                    else
                    {
                        var message = $"Ambiguous constructor: Failed to choose between {type.Name}{Regex.Match(bestCtor.ToString(), @"\((.*?)\)")} and {type.Name}{Regex.Match(ctorInfo.ToString(), @"\((.*?)\)")}";
                        return new InvalidRegistrationException(message);
                    }
                }
            }

            if (bestCtor == null)
            {
                return new InvalidRegistrationException(
                    $"Failed to select a constructor for {type.FullName}");
            }

            return bestCtor;
        }

        #endregion


        #region Expression Overrides

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = builder.Type.GetTypeInfo();
#else
            var typeInfo = builder.Type;
#endif
            // Validate if Type could be created
            if (typeInfo.IsInterface) return CannotConstructInterfaceExpr.Concat(builder.Express());

            if (typeInfo.IsAbstract) return CannotConstructAbstractClassExpr.Concat(builder.Express());

            if (typeInfo.IsSubclassOf(typeof(Delegate)))
                return CannotConstructDelegateExpr.Concat(builder.Express());

            if (typeof(string) == builder.Type)
                return TypeIsNotConstructableExpr.Concat(builder.Express());

            // Build expression as usual
            return base.Express(ref builder);
        }

        protected override IEnumerable<Expression> GetConstructorExpression(ConstructorInfo info, object? resolvers)
        {
            var parameters = info.GetParameters();
            var variables = parameters.Select(p => Expression.Variable(p.ParameterType, p.Name))
                                        .ToArray();

            // Check if has Out or ByRef parameters
            var tryBlock = parameters.Any(pi => pi.ParameterType.IsByRef)
                
                // Report error
                ? (Expression)Expression.Throw(Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Constant(CreateErrorMessage("The constructor {1} selected for type {0} has ref or out parameters. Such parameters are not supported for constructor injection.", 
                        info.DeclaringType, info))))

                // Create new instance
                : Expression.Assign(
                    PipelineContextExpression.Existing, 
                    Expression.Convert(Expression.New(info, CreateDiagnosticParameterExpressions(info.GetParameters(), resolvers)), typeof(object)));

            //
            var thenBlock = Expression.Block(variables, CreateParameterExpressions(variables, parameters, resolvers)
                                                       .Concat(new[] { Expression.Assign(
                                                           PipelineContextExpression.Existing,
                                                           Expression.Convert(
                                                               Expression.New(info, variables), typeof(object)))}));

            yield return Expression.IfThen(NullEqualExisting, thenBlock);

            // Add location to dictionary and re-throw
            var catchBlock = Expression.Block(tryBlock.Type,
                Expression.Call(ExceptionDataExpr, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(tryBlock.Type));

            // Create 
            yield return Expression.IfThen(NullEqualExisting,
                                     Expression.TryCatch(tryBlock, Expression.Catch(ExceptionExpr, catchBlock)));
            // Report error
            string CreateErrorMessage(string format, Type type, MethodBase constructor)
            {
                var parameterDescriptions =
                    constructor.GetParameters()
                               .Select(parameter => $"{parameter.ParameterType.FullName} {parameter.Name}");

                return string.Format(format, type.FullName, string.Join(", ", parameterDescriptions));
            }
        }

        #endregion


        #region Resolver Overrides

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            if (null != builder.SeedMethod) return builder.Pipeline();

            var type = builder.Type;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
#else
            var typeInfo = type;
#endif
            // Validate if Type could be created
            if (typeInfo.IsInterface)
            {
                var pipeline = builder.Pipeline();
                return (ref PipelineContext context) =>
                {
                    if (null == context.Existing)
                        throw new InvalidRegistrationException(string.Format(CannotConstructInterface, context.Type));

                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
            }

            if (typeInfo.IsAbstract)
            {
                var pipeline = builder.Pipeline();
                return (ref PipelineContext context) =>
                {
                    if (null == context.Existing)
                        throw new InvalidRegistrationException(string.Format(CannotConstructAbstractClass, context.Type));

                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
            }

            if (typeInfo.IsSubclassOf(typeof(Delegate)))
            {
                var pipeline = builder.Pipeline();
                return (ref PipelineContext context) =>
                {
                    if (null == context.Existing)
                        throw new InvalidRegistrationException(string.Format(CannotConstructDelegate, context.Type));

                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
            }

            if (type == typeof(string))
            {
                var pipeline = builder.Pipeline();
                return (ref PipelineContext context) =>
                {
                    if (null == context.Existing)
                        throw new InvalidRegistrationException(string.Format(TypeIsNotConstructable, context.Type));

                    return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
                };
            }

            return base.Build(ref builder);
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(ConstructorInfo info, object? resolvers, ResolveDelegate<PipelineContext>? pipeline)
        {
            var parameterResolvers = CreateDiagnosticParameterResolvers(info.GetParameters(), resolvers).ToArray();

            return (ref PipelineContext context) =>
            {
                if (null == context.Existing)
                {
                    try
                    {
                        var dependencies = new object[parameterResolvers.Length];
                        for (var i = 0; i < dependencies.Length; i++)
                            dependencies[i] = parameterResolvers[i](ref context);

                        context.Existing = info.Invoke(dependencies);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(Guid.NewGuid(), info);
                        throw;
                    }
                }

                return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
            };
        }

        protected override ResolveDelegate<PipelineContext> GetPerResolveDelegate(ConstructorInfo info, object? resolvers, ResolveDelegate<PipelineContext>? pipeline)
        {
            var parameterResolvers = CreateDiagnosticParameterResolvers(info.GetParameters(), resolvers).ToArray();
            // PerResolve lifetime
            return (ref PipelineContext context) =>
            {
                if (null == context.Existing)
                {
                    try
                    {
                        var dependencies = new object[parameterResolvers.Length];
                        for (var i = 0; i < dependencies.Length; i++)
                            dependencies[i] = parameterResolvers[i](ref context);

                        context.Existing = info.Invoke(dependencies);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(Guid.NewGuid(), info);
                        throw;
                    }

                    context.Set(typeof(LifetimeManager),
                          new RuntimePerResolveLifetimeManager(context.Existing));
                }

                return null == pipeline ? context.Existing : pipeline.Invoke(ref context);
            };
        }

        #endregion
    }
}
