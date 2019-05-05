using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class ErrorHandlerBuilder : PipelineBuilder
    {
        #region Constants

        private const string error = "For more information run UnityContainer in diagnostic mode: new UnityContainer(true)";

        #endregion


        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, Type type, ImplicitRegistration registration)
        {
            throw new NotImplementedException();
        }

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            var pipeline = builder.Pipeline();

            // Nothing to build
            if (null == pipeline) return (ref BuilderContext context) => context.Existing;

            // Optimized exception handling
            return (ref BuilderContext context) =>
            {
                try
                {
                    return pipeline(ref context);
                }
                catch (MemberAccessException ex)
                {
                    context.RequiresRecovery?.Recover();
                    throw new ResolutionFailedException(context.RegistrationType, context.Name, error, ex);
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException)
                {
                    context.RequiresRecovery?.Recover();
                    throw new ResolutionFailedException(context.RegistrationType, context.Name, error, ex);
                }
                catch
                {
                    context.RequiresRecovery?.Recover();
                    throw;
                }
            };
        }
    }
}
