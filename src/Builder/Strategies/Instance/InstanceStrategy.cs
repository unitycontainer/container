using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class InstanceStrategy<TContext> where TContext : IBuilderContext
    {

        public static ResolveDelegate<TContext> DefaultPipeline { get; }
            = (ref TContext context) => context.Existing = 
            null == context.Registration
                ? UnityContainer.NoValue
                : context.Registration.Instance;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InstanceBuilderStrategy(ref TContext context)
        {
            //// TODO: Proper validation
            //Debug.Assert(null == context.Existing);
            //Debug.Assert(null != context.Registration);
            //Debug.Assert(RegistrationCategory.Instance == context.Registration?.Category);

            context.Existing = null == context.Registration
                ? UnityContainer.NoValue
                : context.Registration.Instance;
        }
    }
}
