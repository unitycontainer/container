using Unity.Injection;

namespace Unity.Extension;



public delegate void InjectionInfoProvider<TInjectionInfo, TMemberInfo>(ref TInjectionInfo info)
    where TInjectionInfo : IInjectionInfo<TMemberInfo>;
