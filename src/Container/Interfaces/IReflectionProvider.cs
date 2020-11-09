
namespace Unity.Container
{
    public delegate TReturn ReflectionProvider<TMember, TReturn>(ref TMember info);

    public delegate TReturn ReflectionData<TMember, TReturn>(ref TMember info, object? value);




    public delegate MatchRank MatchProvider<TMember>(ref ReflectionInfo<TMember> info);

    public interface IReflectionProvider<TMember>
    {
        ImportData GetReflectionInfo(ref ImportInfo<TMember> info);
    }
}
