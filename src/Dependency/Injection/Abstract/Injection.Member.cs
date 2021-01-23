using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for objects that can be used to configure what
    /// class members get injected by the container.
    /// </summary>
    public abstract class InjectionMember : ISequenceSegment<InjectionMember>
    {
        /// <summary>
        /// This property triggers mandatory registration build
        /// </summary>
        /// <remarks>
        /// If this property is True, the registration will never map
        /// to other type, it will always build its own pipeline.
        /// </remarks>
        public virtual bool BuildRequired => false;
        
        /// <summary>
        /// Reference to the next member
        /// </summary>
        public InjectionMember? Next { get; set; }

        /// <inheritdoc/>
        object? ISequenceSegment.Next { get => Next; set => Next = (InjectionMember?)value; }

        /// <inheritdoc/>
        public int Length => (Next?.Length ?? 0) + 1;

    }

    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember, 
                                                                IImportProvider, 
                                                                IMatch<TMemberInfo, MatchRank>,
                                                                ISequence<InjectionMember<TMemberInfo, TData>>
                                            where TMemberInfo : MemberInfo
                                            where TData       : class
    {
        #region Constructors

        protected InjectionMember(string name, TData? data)
        {
            Name = name;
            Data = data;
        }

        #endregion


        #region Public API

        /// <summary>
        /// Name of the injected member
        /// </summary>
        /// <remarks>
        /// <para> When injected member is either <see cref="InjectionMethod"/>, <see cref="InjectionField"/>,
        /// or <see cref="InjectionProperty"/> this is the name of respective method, field, or property.</para>
        /// <para>In case of <see cref="InjectionConstructor"/>, Name is always ".ctor"</para>
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Data associated with injection member.
        /// </summary>
        public virtual TData? Data { get; }


        /// <inheritdoc/>
        public override bool BuildRequired => true;


        /// <inheritdoc/>
        public virtual MatchRank Matches(TMemberInfo other)
            => MatchRank.NoMatch;

        /// <inheritdoc/>
        public virtual void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TContext    : IBuilderContext
            where TDescriptor : IImportDescriptor => descriptor.Dynamic = Data;

        #endregion


        #region ISequence

        /// <inheritdoc/>
        InjectionMember<TMemberInfo, TData>? ISequenceSegment<InjectionMember<TMemberInfo, TData>>.Next 
            => (InjectionMember<TMemberInfo, TData>?)Next;


        /// <inheritdoc/>
        object? ISequenceSegment.Next { get => Next; set => Next = (InjectionMember<TMemberInfo, TData>?)value; }


        /// <inheritdoc/>
        InjectionMember<TMemberInfo, TData> ISequence<InjectionMember<TMemberInfo, TData>>.this[int index] 
            => this[index];


        public InjectionMember<TMemberInfo, TData> this[int index] 
            => (0 == index)
            ? this : (Unsafe.As<ISequence<InjectionMember<TMemberInfo, TData>>>(Next) ?? 
                throw new ArgumentOutOfRangeException(nameof(index)))[index - 1];

        #endregion
    }
}
