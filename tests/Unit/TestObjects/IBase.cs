using System;

namespace Unity.Tests.TestObjects
{
    public interface IBase
    {
        IService Service { get; set; }
    }

    public interface ILazyDependency
    {
        Lazy<EmailService> Service { get; set; }
    }

    public class Base : IBase
    {
        [Dependency]
        public IService Service { get; set; }
    }

    public class LazyDependency : ILazyDependency
    {
        [Dependency]
        public Lazy<EmailService> Service { get; set; }
    }

    public class LazyDependencyConstructor
    {
        private Lazy<EmailService> service = null;
        
        public LazyDependencyConstructor(Lazy<EmailService> s)
        {
            service = s;
        }
    }
}