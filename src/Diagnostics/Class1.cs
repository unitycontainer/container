using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Diagnostics
{
#if NET5_0
    public readonly ref struct Ref<T>
    {
        private readonly ByReference<T> _ref;

        public Ref(ref T value)
            => _ref = new ByReference<T>(ref value);

        public ref T Value => ref _ref.Value;
    }
#endif
}
