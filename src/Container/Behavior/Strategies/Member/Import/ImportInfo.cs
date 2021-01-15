
namespace Unity.Container
{
    public struct ImportInfo
    {
        public object       Member;
        public Contract     Contract;

        public ImportData   Value;
        public ImportData   Default;
    }
}
