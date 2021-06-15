namespace Unity.Tests.v5.Override
{
    public class SubjectType1ToInjectForPropertyOverride : ISubjectTypeToInjectForPropertyOverride
    {
        public int X { get; set; }
        public string Y { get; set; }

        [Dependency]
        public IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}