using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    public abstract class InjectionMemberInfo<TMemberInfo> : InjectionMember<TMemberInfo, object>,
                                                             IReflectionProvider<TMemberInfo>
                                         where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly Type? _type;
        private readonly string? _name;
        private readonly bool _optional;

        #endregion


        #region Constructors

        protected InjectionMemberInfo(string member, object data)
            : base(member, data)
        {
        }

        protected InjectionMemberInfo(string member, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, Type contractType, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _type = contractType;
            _optional = optional;
        }


        protected InjectionMemberInfo(string member, Type contractType, string? contractName, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _type = contractType;
            _name = contractName;
            _optional = optional;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Type MemberType(TMemberInfo info);

        public ReflectionInfo<TMemberInfo> GetInfo(TMemberInfo member)
        {
            if (Data is IReflectionProvider<TMemberInfo> provider)
                return provider.GetInfo(member);

            // TODO: Missing name

            var type = MemberType(member);

            return _type switch
            {
                null when Data is Type target && typeof(Type) != type
                        => new ReflectionInfo<TMemberInfo>(member, target, _optional),

                null when !ReferenceEquals(RegistrationManager.NoValue, Data)
                        => Data switch
                        {
                            RegistrationManager.InvalidValue _        => new ReflectionInfo<TMemberInfo>(member, type, _optional),
                            IResolve iResolve                         => new ReflectionInfo<TMemberInfo>(member, type, _optional, (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),
                            ResolveDelegate<PipelineContext> resolver => new ReflectionInfo<TMemberInfo>(member, type, _optional, Data,                                               ImportType.Pipeline),
                            IResolverFactory<TMemberInfo> infoFactory => new ReflectionInfo<TMemberInfo>(member, type, _optional, infoFactory.GetResolver<PipelineContext>(member),   ImportType.Pipeline),
                            IResolverFactory<Type> typeFactory        => new ReflectionInfo<TMemberInfo>(member, type, _optional, typeFactory.GetResolver<PipelineContext>(type),     ImportType.Pipeline),
                            _                                         => new ReflectionInfo<TMemberInfo>(member, type, _optional, Data,                                               ImportType.Value),
                        },

                _ => new ReflectionInfo<TMemberInfo>(member, _type ?? type, _name, _optional),
            };
        }

        #endregion
    }
}
