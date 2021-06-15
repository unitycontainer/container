namespace Unity.Tests.v5.Override
{
    public class SubjectType2ToInjectForPropertyOverride : ISubjectTypeToInjectForPropertyOverride
    {
        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}