
namespace Unity.Injection
{
    public delegate bool MatchDelegate<T>(T other);

    public interface IMatch<T>
    {
        public bool Match(T other);
    }
}
