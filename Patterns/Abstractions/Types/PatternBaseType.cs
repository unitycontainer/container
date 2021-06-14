using System;

namespace Regression
{
    public abstract class PatternBaseType
    {
        #region Properties

        public virtual object Value { get; protected set; }
        public virtual object Default { get; protected set; }

        #endregion
    }
}


