
namespace Unity.Container
{
    public enum ImportType
    {
        None = 0,
        
        MemberFactory,
        
        TypeFactory,

        Provider,
        
        Pipeline,
        
        Value,

        Unknown
    }
}
