using System;

namespace Unity.Extension
{
    public interface IExpressPipeline<TContext>
        where TContext : IBuilderContext
    {
        BuilderStrategy Strategy { get; }
    }
}
