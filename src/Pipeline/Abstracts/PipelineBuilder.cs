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

        public virtual ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder) => builder.Pipeline();

        #endregion
    }
}
