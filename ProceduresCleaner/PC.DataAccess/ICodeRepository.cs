using System.Collections.Generic;
using PC.Common;

namespace PC.DataAccess
{
    public interface ICodeRepository
    {
        IEnumerable<string> GetCodeFilesPaths(string rootPath);

        IEnumerable<ScanResult> SearchFile(string path, IEnumerable<string> searchPatterns);
    }
}
