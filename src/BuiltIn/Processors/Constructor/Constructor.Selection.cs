using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Delegates

        public delegate ConstructorInfo ConstructorSelectorDelegate(ref ResolutionContext context, ConstructorInfo[] members);

        #endregion


        #region Fields

        protected ConstructorSelectorDelegate SelectConstructor;

        #endregion


        private static ConstructorInfo DefaultConstructorSelector(ref ResolutionContext context, ConstructorInfo[] members)
        {
            if (0 < members.Length) return members[0];

            throw new NotImplementedException();
        }
    }
}
