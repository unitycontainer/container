using Regression;
using System;

namespace Dependency
{
    public abstract partial class Pattern 
    {
        public abstract class DependencyBaseType : PatternBaseType
        {
            #region Constructors

            protected DependencyBaseType() => ImportType = typeof(object);
            public DependencyBaseType(Type type) => ImportType = type;

            #endregion


            #region Properties

            public virtual object Injected { get; protected set; }
            public virtual object Override { get; protected set; }
            public virtual object Registered { get; protected set; }
            public virtual Type ImportType { get; protected set; }

            #endregion
        }

        public class DummyImport : DependencyBaseType 
        {
            public override Type ImportType => typeof(Unresolvable);
            public override object Value => RegisteredUnresolvable;
            public override object Default => RegisteredUnresolvable;
        }

        public class DependecyConsumer<TDependency> : PatternBaseType
            where TDependency : PatternBaseType
        {
            public DependecyConsumer(TDependency dependency)
            {
                Value = dependency.Value;
                Default = dependency.Default;
            }
        }
    }
}


