using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public abstract class Pipeline
    {
        #region Public Members

        public virtual IEnumerable<Expression> Build(UnityContainer container, IEnumerator<Pipeline> enumerator, 
                                                      Type type, IRegistration registration) => throw new NotImplementedException();

        public virtual ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder) => builder.Pipeline();

        #endregion
    }
}
