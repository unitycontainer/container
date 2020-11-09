using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public interface IImportInfo
    {
        Type MemberType { get; }
        bool AllowDefault { get; set; }


        Type    ContractType { get; set; }
        string? ContractName { get; set; }


        ImportSource Source { get; set; }
        CreationPolicy Policy { get; set; }


        object?    ImportValue { get; set; }

        ImportType ImportType { get; set; }
    }
}
