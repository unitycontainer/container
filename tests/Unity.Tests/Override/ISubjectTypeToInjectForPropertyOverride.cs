namespace Unity.Tests.v5.Override
{
    public interface ISubjectTypeToInjectForPropertyOverride
    {
        int X { get; set; }
        string Y { get; set; }
        [Dependency]
        IInterfaceForTypesToInjectForPropertyOverride InjectedObject { get; set; }
    }
}