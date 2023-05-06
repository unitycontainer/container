using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

namespace UnityContainer.Tests
{
    public class PatternTestMethodAttribute : TestMethodAttribute
    {
        public PatternTestMethodAttribute(string pattern, [CallerMemberName] string name = null)
            : base(string.Format(pattern, name.Split('_')))
        {
        }
    }
}
