using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {Type.Name},  Name: {Name}")]
    public partial struct PipelineContext : IBuilderContext
    {
        #region Fields

        private readonly IntPtr _error;
        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _registration;
        private readonly bool   _perResolve;
        
        private IntPtr  _contract;
        private object? _target;

        public UnityContainer Container { get; private set; }
        public RegistrationManager? Registration { get; set; }

        #endregion


        #region Public Properties

        public Type Type { get => Registration?.Type ?? Contract.Type; }

        public string? Name
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Name;
                }
            }
        }

        public object? Action { get; set; }




        public object? Get(Type? type, Type policy)
        {
            throw new NotImplementedException();
        }

        public void Set(Type? type, Type policy, object instance)
        {
            throw new NotImplementedException();
        }

        public void Clear(Type? type, Type policy)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
