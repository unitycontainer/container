
namespace Unity.Injection
{
    public interface IInjectionProvider
    {
        void GetImportInfo<TInfo>(ref TInfo import)
            where TInfo : IInjectionInfo;
    }
}
