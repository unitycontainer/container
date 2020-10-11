
namespace Unity
{
    /// <summary>
    /// Calculates how much member matches
    /// </summary>
    /// <typeparam name="T">What kind of match</typeparam>
    public interface IMatch<T>
    {
        public MatchRank Match(T other);
    }
}
