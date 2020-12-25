using System;
using Unity.Extension;

namespace Unity.Benchmarks
{
    /// <summary>
    /// An extension to install custom strategy that disables
    /// saving of created build plan
    /// </summary>
    public class PipelineSpyExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            //Context.Strategies.Add(new PipelineSpyStrategy(), UnityBuildStage.PreCreation);
        }
    }
}
