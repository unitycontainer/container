
namespace Unity.Injection
{
    public interface IMatching<T>
    {
        public bool Matching(T other);
    }
}
