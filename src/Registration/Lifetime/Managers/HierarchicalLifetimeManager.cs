using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Unity container allows creating hierarchies of child containers. This lifetime 
    /// creates local singleton for each level of the hierarchy. So, when you resolve a 
    /// type and this container does not have an instance of that type, the container will 
    /// create new instance. Next type the type is resolved the same instance will be returned.
    /// </para>
    /// <para>
    /// If a child container is created and requested to resolve the type, the child container 
    /// will create a new instance and store it for subsequent resolutions. Next time the 
    /// child container requested to resolve the type, it will return stored instance.
    /// </para>
    /// <para>If you have multiple children, each will resolve its own instance.</para>
    /// </remarks>
    public class HierarchicalLifetimeManager : SynchronizedLifetimeManager, 
                                               IFactoryLifetimeManager,
                                               ITypeLifetimeManager
    {
        #region Fields

        private int       _index;
        private Entry[]    _data;
        private Metadata[] _meta;

        #endregion


        #region Constructors

        public HierarchicalLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
            _data = new Entry[5];
            _meta = new Metadata[7];
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? TryGetValue(ICollection<IDisposable> scope) 
            => SynchronizedGetValue(scope);

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> scope)
        {
            var meta = _meta;
            var position = meta[((uint)scope.GetHashCode()) % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                var target = candidate.Weak?.Target;
                if (ReferenceEquals(scope, target))
                    return candidate.Value;

                position = meta[position].Location;
            }

            return NoValue;
        }


        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? value, ICollection<IDisposable> scope)
        {
            var hash = scope.GetHashCode();
            var target = ((uint)hash) % _meta.Length;
            var position = _meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (hash == candidate.HashCode)
                { 
                    var key = candidate.Weak!.Target;
                    if (key is null)
                    {
                        candidate.Weak!.Target = scope;
                        candidate.Value = value;
                        goto AddToScope;
                    }

                    if (ReferenceEquals(scope, key))
                    {
                        candidate.Value = value;
                        goto AddToScope;
                    }
                }

                position = _meta[position].Location;
            }

            // Nothing is found, add new and expand if required
            if (_data.Length <= ++_index)
            {
                var prime = Prime.NextUp(_index);
                
                Array.Resize(ref _data, Prime.Numbers[prime++]);
                var meta = new Metadata[Prime.Numbers[prime]];

                for (var current = 1; current < _index; current++)
                {
                    target = ((uint)_data[current].HashCode) % meta.Length;
                    meta[current].Location = meta[target].Position;
                    meta[target].Position = current;
                }

                _meta = meta;
                target = ((uint)hash) % _meta.Length;
            }

            ref var bucket = ref _meta[target];
            ref var data = ref _data[_index];
            
            data.HashCode = hash;
            data.Value = value;
            data.Weak = new WeakReference(scope);
            
            _meta[_index].Location = bucket.Position;
            bucket.Position = _index;
            
            AddToScope: if (value is IDisposable disposable) scope.Add(disposable);
        }

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.OnceInWhile;

        /// <inheritdoc/>
        public override ImportSource Source => ImportSource.Local;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new HierarchicalLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() => "Lifetime:Hierarchical";

        #endregion


        #region Nested Types

        public struct Entry
        {
            public int HashCode;
            public WeakReference? Weak;
            public object? Value;
        }


        #endregion
    }
}
