namespace Unity.Injection;



public interface IInjectionInfoProvider<TMemberInfo> : IInjectionInfoProvider
{
    new void ProvideInfo<TInjectionInfo>(ref TInjectionInfo info)
        where TInjectionInfo : IInjectionInfo<TMemberInfo>;
}
