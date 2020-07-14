using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Constants

        protected const float LoadFactor = 0.72f;

        #endregion


        #region Fields

        protected int _version;
        protected readonly int _level;
        protected readonly Scope? _parent;
        protected readonly ICollection<IDisposable> _disposables;

        #endregion


        #region Constructors

        /// <summary>
        /// Root scope
        /// </summary>
        protected internal Scope()
        {
            _level  = 1;
            _parent = null;
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="parent">Possible parent scope</param>
        /// <param name="disposables">Copy of disposables</param>
        protected internal Scope(Scope? parent, ICollection<IDisposable> disposables)
        {
            _level  = (parent?._level ?? 0) + 1;
            _parent = parent;
            _disposables = disposables;
        }

        /// <summary>
        /// Child scope
        /// </summary>
        /// <param name="parent">parent scope</param>
        protected Scope(Scope parent)
        {
            // Copy data
            _level  = parent._level + 1;
            _parent = parent;
            _disposables = new List<IDisposable>();
        }

        #endregion
    }
}
