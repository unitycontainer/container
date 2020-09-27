
namespace Unity
{
    /// <summary>
    /// Calculates how much member matches
    /// </summary>
    /// <typeparam name="T">What kind of match</typeparam>
    public interface IMatchTo<T>
    {
        public MatchRank MatchTo(T other);
    }
}
