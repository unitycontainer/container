using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo ArrayMethod =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeArray))!;

        private static readonly MethodInfo ArrayWithTargetedTypes =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeTargetedArray))!;

        private static readonly MethodInfo ArrayWithGenericTarget =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeGenericArray))!;


        private static readonly MethodInfo EnumeratorMethod =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeEnumerator))!;

        private static readonly MethodInfo EnumeratorWithTargetedTypes =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeEnumeratorTarget))!;

        private static readonly MethodInfo EnumeratorWithGenericTarget =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(MakeEnumeratorGeneric))!;

        #endregion


        #region Array

        private static TElement[] MakeArray<TElement>(ref PipelineContext context) 
            => GetEnumerator<TElement>(context.Container, context.Contract.Name, false).ToArray();

        private static TElement[] MakeTargetedArray<TElement, TTarget>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget>(context.Container, context.Contract.Name, false).ToArray();

        private static TElement[] MakeGenericArray<TElement, TTarget, TGeneric>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget, TGeneric>(context.Container, context.Contract.Name, false).ToArray();

        #endregion


        #region Enumerable

        private static IEnumerable<TElement> MakeEnumerator<TElement>(ref PipelineContext context)
            => GetEnumerator<TElement>(context.Container, context.Contract.Name, true);

        private static IEnumerable<TElement> MakeEnumeratorTarget<TElement, TTarget>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget>(context.Container, context.Contract.Name, true);

        private static IEnumerable<TElement> MakeEnumeratorGeneric<TElement, TTarget, TGeneric>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget, TGeneric>(context.Container, context.Contract.Name, true);

        #endregion


        #region Implementation

        private static IEnumerable<TElement> GetEnumerator<TElement>(UnityContainer container, string? name, bool allowDefault)
        {
            UnityContainer? scope = container;

            do
            {
                var enumerator = container._scope.GetEnumerator(typeof(TElement));
                while (enumerator.MoveNext(true))
                {
                    var type = enumerator.Current.Contract.Type;
                    var nname = enumerator.Current.Contract.Name;
                }
                
                if (!enumerator.HasNamed) continue;

                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                    var type = enumerator.Current.Contract.Type;
                    var nname = enumerator.Current.Contract.Name;
                }
            }
            while (null != (scope = scope.Parent));

            yield break;
        }

        private static IEnumerable<TElement> GetEnumerator<TElement, TTarget>(UnityContainer container, string? name, bool allowDefault)
        {
            UnityContainer? scope = container;

            do
            {

            }
            while (null != (scope = scope.Parent));

            yield break;
        }


        private static IEnumerable<TElement> GetEnumerator<TElement, TTarget, TGeneric>(UnityContainer container, string? name, bool allowDefault)
        {
            UnityContainer? scope = container;

            do
            {

            }
            while (null != (scope = scope.Parent));

            yield break;
        }

        #endregion
    }
}
