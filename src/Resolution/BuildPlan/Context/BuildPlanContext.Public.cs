using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Storage;

namespace Unity.Resolution
{
    public partial struct BuildPlanContext<TTarget>
    {
        public BuildPlanContext(ref BuilderContext parent)
        {
            unsafe
            {
                _parent       = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            TargetType    = parent.Type;
            Container     = parent.Container;

            _error        = parent._error;
            _registration = parent._registration;
        }


        public void Error(string error)
        {
            unsafe
            {
                Unsafe.AsRef<ErrorDescriptor>(_error.ToPointer())
                      .Error(error);
            }
        }

        public object Capture(Exception exception)
        {
            unsafe
            {
                return Unsafe.AsRef<ErrorDescriptor>(_error.ToPointer())
                             .Capture(exception);
            }
        }
    }
}
