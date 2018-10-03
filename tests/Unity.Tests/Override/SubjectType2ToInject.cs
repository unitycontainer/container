namespace Unity.Tests.v5.Override
{
    public class SubjectType2ToInject : ISubjectTypeToInject
    {
        [InjectionConstructor]
        public SubjectType2ToInject(IInterfaceForTypesToInject injectedObject)
        {
            InjectedObject = injectedObject;
        }

        public SubjectType2ToInject(int x, string y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInject InjectedObject { get; set; }
    }
}