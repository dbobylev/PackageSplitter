using OracleParser.Model.PackageModel;

namespace PackageSplitter.Model.Split
{
    public interface IOracleParsedPackageSetter
    {
        void SetOracleParsedPackage(Package repositoryPackage);
    }
}
