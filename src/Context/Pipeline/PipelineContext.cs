using System;
using System.Text.RegularExpressions;
using Unity.Resolution;

namespace Unity
{
    public struct PipelineContext
    {
        #region Public Properties

        public bool    RunAsync;
        public Type    Type;
        public string? Name;
        public Regex   Regex;
        public object? Existing;
        public bool    RunningAsync;
        public ResolverOverride[] Overrides;
        public UnityContainer.ContainerContext ContainerContext;

        #endregion
    }
}
