using System;

namespace Unity.Builder.Operation
{
    public class ParameterResolveOperation : BuildOperation
    {
        public ParameterResolveOperation(Type typeBeingConstructed, string parameterName) 
            : base(typeBeingConstructed)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Argument that's being resolved.
        /// </summary>
        public string ParameterName { get; }
    }
}
