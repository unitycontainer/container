using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public class ConstructorDiagnostic : ConstructorProcessor
    {
        #region Fields

        const string CannotConstructAbstractClass = "The current type, {0}, is an abstract class and cannot be constructed. Are you missing a type mapping?";
        const string CannotConstructDelegate = "The current type, {0}, is delegate and cannot be constructed. Unity only supports resolving Func&lt;T&gt; and Func&lt;IEnumerable&lt;T&gt;&gt; by default.";
        const string CannotConstructInterface = "The current type, {0}, is an interface and cannot be constructed. Are you missing a type mapping?";
        const string TypeIsNotConstructable = "The type {0} cannot be constructed. You must configure the container to supply this value.";

        private static readonly Expression[] CannotConstructInterfaceExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructInterface),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)))};

        private static readonly Expression[] CannotConstructAbstractClassExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructAbstractClass),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)))};

        private static readonly Expression[] CannotConstructDelegateExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(CannotConstructDelegate),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)))};

        private static readonly Expression[] TypeIsNotConstructableExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(TypeIsNotConstructable),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)))};

        #endregion


        #region Constructors

        public ConstructorDiagnostic(IPolicySet policySet, UnityContainer container) 
            : base(policySet, container)
        {
        }

        #endregion


        #region Selection

        protected override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            var members = new List<InjectionMember>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
                    {
                        members.Add(injectionMember);
                    }
                }
            }

            switch (members.Count)
            {
                case 1:
                    return members[0];

                case 0:
                    break;

                default:
                    return new InvalidOperationException($"Multiple Injection Constructors are registered for Type {type.FullName}",
                           new InvalidRegistrationException());
            }

            // Enumerate to array
            var constructors = DeclaredMembers(type).ToArray();


            // No constructors
            if (0 == constructors.Length)
            {
                return new InvalidOperationException($"No public constructor is available for type {type.FullName}.",
                    new InvalidRegistrationException());
            }

            // Just one constructor
            if (1 == constructors.Length) return constructors[0];

            var selection = new HashSet<ConstructorInfo>();

            // Select Attributed constructors
            foreach (var constructor in constructors)
            {
                if (constructor.IsDefined(typeof(InjectionConstructorAttribute)))
                    selection.Add(constructor);
            }

            switch (selection.Count)
            {
                case 1:
                    return selection.First();

                case 0:
                    break;

                default:
                    return new InvalidOperationException($"Multiple Constructors are annotated for injection on Type {type.FullName}",
                           new InvalidRegistrationException());
            }

            return SelectMethod(type, constructors) ??
                new InvalidOperationException($"Unable to select constructor for type {type.FullName}.",
                    new InvalidRegistrationException());
        }

        protected override object SmartSelector(Type type, ConstructorInfo[] constructors)
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
                        return new InvalidOperationException(message, new InvalidRegistrationException());
                    }
                }
            }

            if (bestCtor == null)
            {
                return new InvalidOperationException(
                    $"Failed to select a constructor for {type.FullName}", new InvalidRegistrationException());
            }

            return bestCtor;
        }

        #endregion


        #region Expression Overrides

        public override IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
#else
            var typeInfo = type;
#endif
            // Validate if Type could be created
            if (typeInfo.IsInterface) return CannotConstructInterfaceExpr;

            if (typeInfo.IsAbstract) return CannotConstructAbstractClassExpr;

            if (typeInfo.IsSubclassOf(typeof(Delegate)))
                return CannotConstructDelegateExpr;

            if (type == typeof(string))
                return TypeIsNotConstructableExpr;

            // Build expression as usual
            return base.GetExpressions(type, registration);
        }

        #endregion


        #region Resolver Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext>? seed)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
#else
            var typeInfo = type;
#endif
            // Validate if Type could be created
            if (typeInfo.IsInterface)
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(CannotConstructInterface, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            if (typeInfo.IsAbstract)
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(CannotConstructAbstractClass, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            if (typeInfo.IsSubclassOf(typeof(Delegate)))
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(CannotConstructDelegate, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            if (type == typeof(string))
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(TypeIsNotConstructable, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }


            return base.GetResolver(type, registration, seed);
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return (ref BuilderContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
                return (ref BuilderContext context) => context.OverrideDiagnostic(info, attribute.Name, data);
            else
                return (ref BuilderContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        #endregion
    }
}
