namespace Unit.Test.Override
{
    public class SubjectType2ToInjectForPropertyOverride : ISubjectTypeToInjectForPropertyOverride
    {
        public int X { get; set; }
        public string Y { get; set; }
        public IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}