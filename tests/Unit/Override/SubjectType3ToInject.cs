namespace Unity.Tests.v5.Override
{
    public class SubjectType3ToInject : ISubjectTypeToInject
    {
        public SubjectType3ToInject(int x, string y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInject InjectedObject { get; set; }
    }
}