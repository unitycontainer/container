using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        #region Default Chain Factory

        public static ResolveDelegate<TContext> BuildUp<TContext>(IEnumerable strategies)
            where TContext : IBuilderContext
        {
            var processors = ((IEnumerable<BuilderStrategy>)strategies).ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }

        #endregion
    }
}
