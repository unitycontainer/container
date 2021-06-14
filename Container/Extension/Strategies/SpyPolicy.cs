#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
#endif

namespace Regression.Container
{
    /// <summary>
    /// A sample policy that gets used by the SpyStrategy
    /// if present to mark execution.
    /// </summary>
#if UNITY_V4
    public class SpyPolicy : IBuilderPolicy
#else
    public class SpyPolicy
#endif
    {
        private bool _spied;

        public bool WasSpiedOn
        {
            get { return _spied; }
            set { _spied = value; }
        }
    }
}
