using Regression;
using System;
using System.Reflection;

namespace Injection
{
    public abstract partial class Pattern : PatternBase
    {
        #region Properties

        protected static Type BaselineConsumer;
        protected static Type NoPublicMember;
        protected static Type BaselineArrayType;
        protected static Type BaselineTestNamed;
        protected static Type BaselineTestType;

        #endregion


        #region Scaffolding

        public static void Pattern_Initialize(string name, Assembly assembly = null)
        {
            PatternBaseInitialize(name);
        
            NoPublicMember = GetTestType("NoPublicMember`1");
            BaselineTestType = GetTestType("BaselineTestType`1");
            BaselineConsumer = GetTestType("BaselineConsumer`1");
            BaselineArrayType = GetTestType("BaselineArrayType`1");
            BaselineTestNamed = GetTestType("BaselineTestTypeNamed`1");
        }

        #endregion
    }
}
