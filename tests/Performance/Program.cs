using BenchmarkDotNet.Running;
using System.Reflection;
using Unity;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            //var container = new UnityContainer();

            //var res = container.Resolve(typeof(IUnityContainer), null, null);

            //if (0 == args.Length)
            //    BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).RunAllJoined();
            //else
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
