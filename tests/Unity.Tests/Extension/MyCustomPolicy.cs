using Unity.Builder;
using Unity.Policy;

namespace Unity.Tests.v5.Extension
{
    internal class MyCustomPolicy : IBuildPlanPolicy
    {
        public bool MyCustomPolicyBool { get; set; }

        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(ref BuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new object();
            }
        }
    }
}
