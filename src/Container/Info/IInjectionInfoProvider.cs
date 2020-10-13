using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Container
{
    public interface IInjectionInfoProvider
    {
    }

    public interface IInjectionInfoProvider<TInfo> : IInjectionInfoProvider
    {
        InjectionInfo<TInfo> GetInfo(TInfo member);
    }
}
