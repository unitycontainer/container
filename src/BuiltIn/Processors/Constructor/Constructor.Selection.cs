using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Delegates

        public delegate object? CtorSelectorDelegate(ConstructorInfo[] members);

        #endregion


        #region Selection

        //public override object Select(ref PipelineContext context)
        //{
        //    throw new NotImplementedException();

        //    //// Select Injected Members
        //    //if (null != builder.InjectionMembers)
        //    //{
        //    //    foreach (var injectionMember in builder.InjectionMembers)
        //    //    {
        //    //        if (injectionMember is InjectionMember<ConstructorInfo, object[]>)
        //    //        {
        //    //            return injectionMember;
        //    //        }
        //    //    }
        //    //}

        //    //// Enumerate to array
        //    //var constructors = DeclaredMembers(builder.Type).ToArray();

        //    //// No constructors
        //    //if (0 == constructors.Length)
        //    //    return new InvalidRegistrationException($"Type {builder.Type.FullName} has no accessible constructors");

        //    //// One constructor
        //    //if (1 == constructors.Length)
        //    //    return constructors[0];

        //    //// Check for constructors decorated with attribute
        //    //foreach (var constructor in constructors)
        //    //{
        //    //    if (!constructor.IsDefined(typeof(InjectionConstructorAttribute), true))
        //    //        continue;

        //    //    return constructor;
        //    //}

        //    //return SelectMethod(ref builder, constructors) ?? throw new InvalidOperationException($"Unable to select constructor for type {builder.Type.FullName}.");
        //}

        ///// <summary>
        ///// Selects default constructor
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to be built</param>
        ///// <param name="members">All public constructors this type implements</param>
        ///// <returns></returns>
        //public virtual object? LegacySelector(ref PipelineBuilder builder, ConstructorInfo[] members)
        //{
        //    Array.Sort(members, (x, y) => y?.GetParameters().Length ?? 0 - x?.GetParameters().Length ?? 0);

        //    switch (members.Length)
        //    {
        //        case 0:
        //            return null;

        //        case 1:
        //            return members[0];

        //        default:
        //            var paramLength = members[0].GetParameters().Length;
        //            if (members[1].GetParameters().Length == paramLength)
        //            {
        //                return new InvalidRegistrationException(
        //                    string.Format(
        //                        CultureInfo.CurrentCulture,
        //                        "The type {0} has multiple constructors of length {1}. Unable to disambiguate.",
        //                        builder.Type.GetTypeInfo().Name,
        //                        paramLength));
        //            }
        //            return members[0];
        //    }
        //}

        //protected virtual object? SmartSelector(ref PipelineBuilder builder, ConstructorInfo[] constructors)
        //{
        //    Array.Sort(constructors, (left, right) =>
        //    {
        //        var qtd = right.GetParameters().Length.CompareTo(left.GetParameters().Length);

        //        if (qtd == 0)
        //        {
        //            return right.GetParameters()
        //                    .Sum(p => p.ParameterType.IsInterface() ? 1 : 0)
        //                    .CompareTo(left.GetParameters()
        //                                   .Sum(p => p.ParameterType.IsInterface() ? 1 : 0));
        //        }
        //        return qtd;
        //    });

        //    UnityContainer container = builder.Container; 
        //    foreach (var ctorInfo in constructors)
        //    {
        //        var parameters = ctorInfo.GetParameters();
        //        if (parameters.All(p => p.HasDefaultValue || CanResolve(container, p)))
        //        {
        //            return ctorInfo;
        //        }
        //    }

        //    return new InvalidRegistrationException($"Failed to select a constructor for {builder.Type.FullName}");
        //}

        #endregion


        #region Implementation

        protected bool CanResolve(UnityContainer container, ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute;

            if (null != attribute) return container.CanResolve(info.ParameterType, attribute.Name);

            return container.CanResolve(info.ParameterType, null);
        }

        #endregion
    }
}
