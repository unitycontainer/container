using System;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private Metadata[] GetRegistrations<TTarget, TGeneric>(bool anonymous)
        {
            var size = _depth + 2;
            var set = new RegistrationSet(_scope, size);
            Span<Metadata> stack = stackalloc Metadata[size];

            var enumerator = _scope.GetEnumerator<TTarget>(anonymous, in stack);
            while (enumerator.MoveNext())
            {
                var manager = enumerator.Manager;

                if (null == manager || RegistrationCategory.Internal > manager.Category) 
                    continue;

                set.Add(in enumerator);
            }

            return set.GetRecording();
        }
    }
}
