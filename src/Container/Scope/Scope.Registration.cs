using System;
using System.Diagnostics;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {
            public bool IsExplicitlyRegistered(Type type, string? name)
            {
                return false;
            }


            #region Registration Entry

            [DebuggerDisplay("{ (null == Type ? string.Empty : Name),nq }", 
                      Name = "{ (Type?.Name ?? string.Empty),nq }")]
            public struct Registry
            {
                public int HashCode;
                public Type    Type;
                public string? Name;
                public RegistrationManager Manager;

                public override int GetHashCode() 
                    => ((Type.GetHashCode() + 37) ^ ((Name?.GetHashCode() ?? 0) + 17)) & HashMask;
            }

            #endregion
        }
    }
}
