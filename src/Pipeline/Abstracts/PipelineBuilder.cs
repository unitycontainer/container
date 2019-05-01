using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract class PipelineBuilder
    {
        #region Public Members

        public abstract IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                      Type type, ImplicitRegistration registration);

        public abstract ResolveDelegate<BuilderContext>? Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator,
                                                              Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed);

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext>? Pipeline(UnityContainer container, IEnumerator<PipelineBuilder> enumerator,
                                                                   Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed) 
            => enumerator.MoveNext() ? enumerator.Current.Build(container, enumerator, type, registration, seed) : seed;

        #endregion
    }
}
