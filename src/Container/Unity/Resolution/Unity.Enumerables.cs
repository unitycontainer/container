using System.Collections.Generic;
using System.Reflection;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo EnumeratorMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(ElementEnumerator))!;

        private static readonly MethodInfo EnumeratorGeneric =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(GenericEnumerator))!;

        #endregion

        #region Enumerable

        private static IEnumerable<TElement> ElementEnumerator<TElement, TTarget>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget>(context.Container, context.Contract.Name, true);

        private static IEnumerable<TElement> GenericEnumerator<TElement, TTarget, TGeneric>(ref PipelineContext context)
            => GetEnumerator<TElement, TTarget, TGeneric>(context.Container, context.Contract.Name, true);

        #endregion


        #region Implementation


        private static IEnumerable<TElement> GetEnumerator<TElement, TTarget>(UnityContainer container, string? name, bool includeDefault)
        {
            var iterator = container._scope.GetIterator(typeof(TTarget), includeDefault);
            while (iterator.MoveNext())
            {
                var type  = iterator.Current.Contract.Type;
                var nname = iterator.Current.Contract.Name;


            }

            yield break;
        }

        private static IEnumerable<TElement> GetEnumerator<TElement, TTarget, TGeneric>(UnityContainer container, string? name, bool includeDefault)
        {
            var iterator = container._scope.GetIterator(typeof(TTarget), includeDefault);
            while (iterator.MoveNext())
            {
                var type = iterator.Current.Contract.Type;
                var nname = iterator.Current.Contract.Name;


            }

            iterator = container._scope.GetIterator(typeof(TGeneric), includeDefault);
            while (iterator.MoveNext())
            {
                var type = iterator.Current.Contract.Type;
                var nname = iterator.Current.Contract.Name;


            }

            yield break;
        }

        #endregion
    }
}
