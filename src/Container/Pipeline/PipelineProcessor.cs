using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class PipelineProcessor : BuilderStrategy
    {
        #region Constants

        /// <summary>
        /// Default, empty pipeline implementation
        /// </summary>
        public static readonly ResolveDelegate<PipelineContext> DefaultPipeline = (ref PipelineContext context) => context.Target;


        #endregion


        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref Pipeline_Builder<ResolveDelegate<PipelineContext>?> builder) => builder.Build();

        public virtual IEnumerable<Expression> Express(ref Pipeline_Builder<IEnumerable<Expression>> builder) => builder.Express();

        #endregion


        #region Import

        public static void ProcessImport<T>(ref T info, object? value)
            where T : IInjectionInfo
        {
            do
            {
                switch (value)
                {
                    case IInjectionProvider provider:
                        provider.GetImportInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.Pipeline = iResolve.Resolve;
                        return;

                    case ResolveDelegate<PipelineContext> resolver:
                        info.Pipeline = resolver;
                        return;

                    case IResolverFactory<Type> typeFactory:
                        info.Pipeline = typeFactory.GetResolver<PipelineContext>(info.MemberType);
                        return;

                    case PipelineFactory<PipelineContext> factory:
                        info.Pipeline = factory(info.MemberType);
                        return;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.AllowDefault = false;
                        return;

                    case UnityContainer.InvalidValue _:
                        return;

                    default:
                        info.Value = value;
                        return;
                }

                value = info.ImportValue;
            }
            while (ImportType.Unknown == info.ImportType);
        }

        #endregion
    }
}
