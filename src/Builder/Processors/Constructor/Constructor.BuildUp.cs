namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        public override void BuildUp(ref TContext context)
        {
            // Do nothing if building up
            if (null != context.Existing) return;

            var members = GetDeclaredMembers(context.Type);

            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {context.Type}");
                return;
            }

            var ctorInfo = SelectConstructor(ref context, members);

            try
            {
                if (context.IsFaulted) return;
                BuildUp(ref context, ref ctorInfo);
                
                if (context.IsFaulted) return;
                context.Instance = ctorInfo.MemberInfo.Invoke((object[]?)ctorInfo.DataValue.Value);
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
