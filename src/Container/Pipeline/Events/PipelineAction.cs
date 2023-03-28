using System;
using System.Runtime.CompilerServices;
using Unity.Builder;

namespace Unity.Container
{
    public ref struct PipelineAction<T> 
        where T : class
    {
        #region Fields

        private readonly IntPtr  _parent;
        private readonly object? _backup;

        #endregion


        #region Constructors

        internal PipelineAction(IntPtr parent, T action)
        {
            _parent = parent;
            _backup = action;
        }

        internal PipelineAction(ref BuilderContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            _backup = parent.CurrentOperation;
        }

        internal PipelineAction(ref BuilderContext parent, T? action)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            _backup = parent.CurrentOperation;
            parent.CurrentOperation = action;
        }

        #endregion

        public readonly ref BuilderContext Context
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<BuilderContext>(_parent.ToPointer());
                }
            }
        }

        public T? Current
        {
            get
            {
                unsafe
                {
                    return (T?)Unsafe.AsRef<BuilderContext>(_parent.ToPointer()).CurrentOperation;
                }
            }
            set => Context.CurrentOperation = value;
        }

        public object? Target
        {
            get => Context.Existing;
            set => Context.Existing = value;
        }

        public void Error(string error)
        {
            // Report Error
            Context.Error(error);
        }

        public void Exception(Exception ex)
        {
            // Report exception
            Context.Capture(ex);
        }

        public bool Success(object? data)
        {
            // Report stop
            Context.Existing = data;
            Context.CurrentOperation = _backup;

            return true;
        }

        public void Dispose()
        {
            // Report stop
            Context.CurrentOperation = _backup;
        }
    }
}
