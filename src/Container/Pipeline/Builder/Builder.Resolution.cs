using System;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext> : IBuildPipeline<TContext>
    {
        #region IBuildPipeline

        public ResolveDelegate<TContext> BuildPipeline(object?[] analytics)
        {
            _analytics = analytics;
            
            return Build() ?? UnityContainer.DummyPipeline;
        }

        public ResolveDelegate<TContext>? Build()
        {
            if (_strategies.Length <= _index) return null;

            var analytics = _analytics?[_index];
            var strategy = _strategies[_index++];

            return strategy.BuildPipeline<PipelineBuilder<TContext>, TContext>(ref this, analytics);
        }

        #endregion
    }
}
