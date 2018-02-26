using System;
using Unity;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            var res = container.Resolve(typeof(IUnityContainer), null, null);
        }
    }
}
