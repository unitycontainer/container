namespace Unity.Tests.Lifetime
{
    public class UnityTestClass
    {
        private string name = "Hello";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}