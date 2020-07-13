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
        protected readonly UnityContainer _container;
        protected readonly ICollection<IDisposable> _disposables;

        #endregion


        #region Constructors

        protected internal Scope(UnityContainer container)
        {
            _container = container;
            _parent    = container.Parent?._scope;

            // Scope specific
            _level = null == _parent ? 1 : _parent._level + 1;
            _disposables = new List<IDisposable>();
        }

        #endregion


        // Copy constructor
        protected Scope(Scope scope)
        {
            // Copy data
            _version     = scope._version;
            _level       = scope._level;
            _parent      = scope._parent;
            _container   = scope._container;
            _disposables = scope._disposables;
        }
    }
}
