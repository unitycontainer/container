namespace Unity.Processors
{
    public partial class ConstructorProcessor
    {
        public override void BuildUp<TContext>(ref TContext context)
        {
            // Do nothing if building up
            if (null != context.Existing) return;

            var members = GetDeclaredMembers(context.TargetType);

            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {context.TargetType}");
                return;
            }

            var ctorInfo = SelectConstructor(ref context, members);

            try
            {
                if (context.IsFaulted) return;

                BuildUpParameters(ref context, ref ctorInfo);
                
                if (context.IsFaulted) return;
                context.Existing = ctorInfo.MemberInfo.Invoke((object[]?)ctorInfo.InjectedValue.Value);
            }
            catch (Exception ex) when (ex is ArgumentException || 
                                       ex is MemberAccessException)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }
    }
}
