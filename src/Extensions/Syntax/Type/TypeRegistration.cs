using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Extensions.Syntax
{
    public partial class TypeRegistration
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal IUnityContainer container;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Type type;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal ITypeLifetimeManager lifetime;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal IEnumerable<InjectionMember> members = Enumerable.Empty<InjectionMember>();

        public TypeRegistration(IUnityContainer container, Type type)
        {
            this.container = container;
            this.type = type;

            Lifetime = new LifetimeProxy(this);
        }

        public readonly LifetimeProxy Lifetime;

        #region Object

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        #endregion
    }
}
