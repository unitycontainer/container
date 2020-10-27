using System.Collections.Generic;
using System.Reflection;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo EnumeratorMethod;
        private static readonly MethodInfo EnumeratorGeneric;

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
            yield break;
        }

        private static IEnumerable<TElement> GetEnumerator<TElement, TTarget, TGeneric>(UnityContainer container, string? name, bool includeDefault)
        {
            yield break;
        }

        #endregion
    }
}
