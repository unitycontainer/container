namespace Unity.Tests.v5.Override
{
    public interface ISubjectTypeToInject
    {
        int X { get; set; }
        string Y { get; set; }
        IInterfaceForTypesToInject InjectedObject { get; set; }
    }
}