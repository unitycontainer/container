namespace Unit.Test.TestObjects
{
    internal class ObjectWithMarkedConstructor
    {
        public ObjectWithMarkedConstructor(int notTheInjectionConstructor)
        {
        }

        [InjectionConstructor]
        public ObjectWithMarkedConstructor(string theInjectionConstructor)
        {
        }
    }
}
