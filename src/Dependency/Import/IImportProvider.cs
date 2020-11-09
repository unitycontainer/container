namespace Unity.Container
{
    public delegate ImportType ImportProvider<TImport>(ref TImport import)
        where TImport : IImportInfo;
    
    public delegate ImportData ImportDataProvider<TImport>(TImport import, object? data);

    public interface IImportProvider
    {
        ImportType GetImportInfo<TImport>(ref TImport import)
            where TImport : IImportInfo;
    }
}
