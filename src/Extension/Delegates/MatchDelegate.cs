
namespace Unity.Extension
{
    /// <summary>
    /// Calculates the match between members
    /// </summary>
    public delegate TMatch MatchDelegate<TLeft, TRight, TMatch>(TLeft left, TRight right);

}
