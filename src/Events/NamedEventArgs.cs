

using System;

namespace Unity.Events
{
    /// <summary>
    /// An EventArgs class that holds a string Name.
    /// </summary>
    public abstract class NamedEventArgs : EventArgs
    {
        private string _name;

        /// <summary>
        /// Create a new <see cref="NamedEventArgs"/> with a null name.
        /// </summary>
        protected NamedEventArgs()
        {
        }

        /// <summary>
        /// Create a new <see cref="NamedEventArgs"/> with the given name.
        /// </summary>
        /// <param name="name">Name to store.</param>
        protected NamedEventArgs(string name)
        {
            _name = name;
        }

        /// <summary>
        /// The name.
        /// </summary>
        /// <value>Name used for this EventArg object.</value>
        public virtual string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
