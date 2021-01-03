using System;

namespace Unity
{
    /// <summary>
    /// Calculates how much member matches
    /// </summary>
    /// <typeparam name="TOther"><see cref="Type"/> of the target to match to</typeparam>
    /// <typeparam name="TMatch">Target to match to</typeparam>
    public interface IMatch<in TOther, out TMatch>
    {
        public TMatch Match(TOther other);
    }
}
