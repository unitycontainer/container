using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Properties
{
    public partial class Resolving_Optional
    {
        #region Validation

#if !BEHAVIOR_V4
        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
#endif
        public void Member_null()
        {
            _ = MemberOverride_ByName(null, this);
        }

        #endregion


        #region Not Supported

        public override void Parameter_Override_ByType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_OnType_ByType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_ByNameType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_OnType_ByNameType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }

        #endregion
    }

    public partial class Resolving_Required
    {
        #region Validation

#if !BEHAVIOR_V4
        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
#endif
        public void Member_null()
        {
            _ = MemberOverride_ByName(null, this);
        }

        #endregion


        #region Not Supported

        public override void Parameter_Override_ByType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_OnType_ByType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_ByNameType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Parameter_Override_OnType_ByNameType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }

        #endregion
    }
}
