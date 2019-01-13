using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldDiagnostic : FieldProcessor
    {
        #region Constructors

        public FieldDiagnostic(IPolicySet policySet) : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                try
                {
                    info.SetValue(context.Existing, context.Resolve(info, context.Name, value));
                    return context.Existing;
                }
                catch (Exception ex)
                {
                    ex.Data.Add(info, context.Name);
                    throw;
                }
            };
        }

        #endregion
    }
}
