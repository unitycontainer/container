using Unity.Container;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor : PipelineProcessor
    {
        #region Constructors

        public LifetimeProcessor(Defaults _)
        {

        }

        #endregion


        #region Visitors

        public override void BuildUpVisitor(ref PipelineBuilder<object?> builder)
        {
            switch (builder.LifetimeManager)
            {
                case SynchronizedLifetimeManager sync:
                    SynchronizedLifetimeHandler(ref builder, sync);
                    break;

                case PerResolveLifetimeManager temp:
                    PerResolveLifetimeHandler(ref builder, temp);
                    break;

                case TransientLifetimeManager _:
                    builder.Build();
                    break;

                case LifetimeManager manager:
                    LifetimeManagerHandler(ref builder, manager);
                    break;

                default:
                    builder.Build();
                    break;
            }
        }


        public override void ResolutionVisitor(ref PipelineBuilder<Pipeline?> builder)
        {
            switch (builder.LifetimeManager)
            {
                case SynchronizedLifetimeManager sync:
                    SynchronizedLifetimeHandler(ref builder, sync);
                    break;

                case PerResolveLifetimeManager temp:
                    PerResolveLifetimeHandler(ref builder, temp);
                    break;

                case TransientLifetimeManager _:
                    builder.Build();
                    break;

                case LifetimeManager manager:
                    LifetimeManagerHandler(ref builder, manager);
                    break;

                default:
                    builder.Build();
                    break;
            }
        }

        #endregion
    }
}
