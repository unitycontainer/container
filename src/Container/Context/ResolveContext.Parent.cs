using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Resolution;

namespace Unity.Container
{
#if NETSTANDARD1_6 || NETCOREAPP1_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    public partial class ResolveContext
    {
        ResolveContext _parent;

        public ResolveContext Parent => _parent;
    }
#else
    public partial struct ResolveContext
    {
        //ByReference<ResolveContext> _parent;

        //public readonly ref ResolveContext Parent => ref _parent.Value;
    }
#endif
}
