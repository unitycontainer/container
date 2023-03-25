using Unity.Builder;
using Unity.Strategies;

namespace Unity.Container
{
    public partial class InstanceStrategy : BuilderStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            //// TODO: Proper validation
            //Debug.Assert(null == context.Existing);
            //Debug.Assert(null != context.Registration);
            //Debug.Assert(RegistrationCategory.Instance == context.Registration?.Category);

            context.Existing = null == context.Registration
                ? UnityContainer.NoValue
                : context.Registration.Instance;
        }


        public static void BuilderStrategyDelegate<TContext>(ref TContext context)
            where TContext : IBuilderContext
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
