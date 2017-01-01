using System.Collections.Generic;
using PC.Common;

namespace PC.DataAccess
{
    public interface ICodeRepository
    {
        IEnumerable<string> GetCodeFilesPaths(string rootPath, string[] excludedFileTypes = null,
            string[] excludedDirectories = null);

        IEnumerable<ScanResult> SearchFile(string path, IEnumerable<string> searchPatterns);
    }
}
