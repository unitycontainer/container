using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Container
{
    public interface IImportInfoProvider
    {
    }

    public interface IImportInfoProvider<TInfo> : IImportInfoProvider
    {
        ImportInfo<TInfo> GetImportInfo(TInfo member);
    }
}
