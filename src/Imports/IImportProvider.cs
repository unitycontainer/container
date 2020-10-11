using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity
{
    public interface IImportProvider<TInfo>
    {
        ImportInfo<TInfo> GetImportInfo(TInfo member);
    }
}
