using System;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Resolution
{
    public partial struct ResolutionContext
    {
        #region IResolveContext

        public readonly Type Type
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Type;
                }
            }
        }

        public readonly string? Name
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Name;
                }
            }
        }

        public object? Resolve(Type type, string? name)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Public 

        //public readonly Defaults Defaults => Container._policies;

        #endregion


        #region Contract

        //public readonly ref Contract Contract
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
        //        }
        //    }
        //}

        #endregion


        #region RequestRoot

        public readonly ResolverOverride[] Overrides
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).Overrides;
                }
            }
        }

        #endregion


        #region Set Result

        public void FromValue(object? value)
        {
            Target = value;
        }

        public void FromError(string error)
        {
            IsFaulted = true;
            _storage = error;
        }

        public void FromException(Exception exception)
        {
            IsFaulted = true;
            _storage = exception;
        }

        #endregion


        #region Errors


        public Exception? Exception => _storage as Exception;


        #endregion



        #region Builder Factories

        public PipelineBuilder<Pipeline?> CreateBuilder(PipelineVisitor<Pipeline?>[] visitors) 
            => new PipelineBuilder<Pipeline?>(ref this, visitors);


        public PipelineBuilder<object?> CreateBuilder(PipelineVisitor<object?>[] visitors) 
            => new PipelineBuilder<object?>(ref this, _target, visitors);

        #endregion
    }
}
