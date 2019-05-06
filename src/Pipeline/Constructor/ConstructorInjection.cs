using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Pipeline
{
    public partial class ConstructorDiagnostic
    {
        public static Func<Type, InjectionMember, ConstructorInfo> InjectionValidatingSelector =
            (Type type, InjectionMember member) =>
            {
                ConstructorInfo? selection = null;
                var ctor = (InjectionMember<ConstructorInfo, object[]>)member;

                if (ctor.IsInitialized) throw new InvalidOperationException("Sharing an InjectionConstructor between registrations is not supported");

                // Select Constructor
                foreach (var info in ctor.DeclaredMembers(type))
                {
                    if (!ctor.Data.MatchMemberInfo(info)) continue;

                    if (null != selection)
                    {
                        throw new ArgumentException(
                            $"Constructor .ctor({ctor.Data.Signature()}) is ambiguous, it could be matched with more than one constructor on type {type?.Name}.");
                    }

                    selection = info;
                }

                // Validate
                if (null != selection) return selection;

                throw new ArgumentException(
                    $"Injected constructor .ctor({ctor.Data.Signature()}) could not be matched with any public constructors on type {type?.Name}.");
            };
    }
}
