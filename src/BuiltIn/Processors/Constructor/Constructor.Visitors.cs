using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Build Up

        public override void BuildUpVisitor(ref PipelineBuilder<object?> builder)
        {
            Type type = builder.Type;
            var ctors = type.GetConstructors(BindingFlags);

            // Check if any constructors are available
            if (0 == ctors.Length)
            {
                NoConstructors(ref builder);
                return;
            }

            // Injected constructor
            if (null != builder.Constructor)
            {
                FromInjectedCtor(ref builder, ctors);
                return;
            }


            builder.Target = this;// new object();
        }


        #endregion


        #region Resolution


        public override void ResolutionVisitor(ref PipelineBuilder<Pipeline?> builder)
        {
            Type type = builder.Type;
            var ctors = type.GetConstructors(BindingFlags);

            // Check if any constructors are available
            if (0 == ctors.Length)
            {
                NoConstructors(ref builder);
                return;
            }

            // Injected constructor
            if (null != builder.Constructor)
            {
                FromInjectedCtor(ref builder, ctors);
                return;
            }

            builder.Target = (ref ResolutionContext c) => c.Target = new object();
        }


        #endregion
    }
}
