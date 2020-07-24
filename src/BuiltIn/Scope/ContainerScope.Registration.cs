using System;
using System.Diagnostics;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        [DebuggerDisplay("Name = { Contract.Name }", Name = "{ (Contract.Type?.Name ?? string.Empty),nq }")]
        public struct Registration
        {
            public readonly uint Hash;
            public readonly Contract Contract;
            public RegistrationManager Manager;

            public Registration(uint hash, Type type, RegistrationManager manager)
            {
                Hash = hash;
                Contract = new Contract(type);
                Manager = manager;
            }

            public Registration(uint hash, Type type, string? name, RegistrationManager manager)
            {
                Hash = hash;
                Contract = new Contract(type, name);
                Manager = manager;
            }
        }
    }
}
