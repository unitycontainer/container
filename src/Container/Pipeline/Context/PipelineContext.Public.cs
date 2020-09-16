using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public partial struct PipelineContext
    {
        public readonly ref ResolutionContext Context
        {
            get
            {
                Debug.Assert(IntPtr.Zero != _context);

                unsafe
                {
                    return ref Unsafe.AsRef<ResolutionContext>(_context.ToPointer());
                }
            }
        }


        #region Lifetime

        public LifetimeManager? Manager => Context.Manager as LifetimeManager;

        #endregion


        #region Exception

        public Exception? Exception
        {
            get => IsFaulted ? (Exception?)Context.Data : null;
            set
            {
                IsFaulted = true;
                Context.Data = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public void Throw(Exception exception)
        {
            IsFaulted = true;
            Context.Data = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        #endregion
    }
}
