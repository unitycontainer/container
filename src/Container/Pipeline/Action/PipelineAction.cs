using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public ref struct PipelineAction<TAction> 
        where TAction : class
    {
        #region Fileds

        private readonly IntPtr  _parent;
        private readonly object? _backup;

        #endregion


        #region Constructors

        internal PipelineAction(ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            _backup = parent.Action;
        }


        #endregion

        public readonly ref PipelineContext Context
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<PipelineContext>(_parent.ToPointer());
                }
            }
        }

        public object? Target
        {
            get => Context.Target;
            set => Context.Target = value;
        }

        public TAction? Action
        {
            get => (TAction?)Context.Action;
            set => Context.Action = value;
        }

        public void Error(string error)
        {
            // Report Error
            Context.Error(error);
        }

        public void Exception(Exception ex)
        {
            // Report exception
            throw new NotImplementedException();
        }

        public bool Success(object? data)
        {
            // Report stop
            Context.Target = data;
            Context.Action = _backup;

            return true;
        }

        public void Dispose()
        {
            // Report stop
            Context.Action = _backup;
        }
    }
}
