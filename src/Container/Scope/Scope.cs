using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Fields

        protected int _version;

        protected readonly int _level;
        protected readonly Scope? _parent;
        protected readonly ICollection<IDisposable> _disposables;

        #endregion


        #region Constructors

        protected internal Scope()
        {
            _level  = 1;
            _parent = null;
            _disposables = new List<IDisposable>();
        }

        #endregion

        protected Scope(Scope scope, int level = -1)
        {
            // Copy data
            _level = (-1 == level) ? scope._level + 1 : level;
            _parent = scope;
            _disposables = new List<IDisposable>();
        }
    }
}
