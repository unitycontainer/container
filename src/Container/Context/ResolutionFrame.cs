using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Unity.Container
{
    [StructLayout(LayoutKind.Explicit)]
    public ref struct ResolutionFrame
    {
        [FieldOffset(0)] 
        public PipelineContext Pipeline;
        
        [FieldOffset(0)] 
        public ResolveContext  Resolve;
    }
}
