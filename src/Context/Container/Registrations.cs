using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext : IEnumerable<IContainerRegistration>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<IContainerRegistration> GetEnumerator()
            {
                var set = GetRegistrations(Container);

                return set;
            }


            private static RegistrationSet GetRegistrations(UnityContainer container)
            {
                var seed = null != container._parent ? GetRegistrations(container._parent)
                                                     : new RegistrationSet();

                if (null == container._metadata) return seed;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                for (var i = 0; i < registry.Count; i++)
                {
                    if (!(registry.Entries[i].Value is ExplicitRegistration registration))
                        continue;

                    seed.Add(registry.Entries[i].Type, registration);
                }

                return seed;
            }


        }
    }
}
