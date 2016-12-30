using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PC.DataAccess
{
    public class StoredProceduresRepository : IStoredProceduresRepository
    {
        public IEnumerable<string> GetStoreProceduresNames(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException(rootPath);

            return Directory.GetFiles(rootPath, ".sql", SearchOption.AllDirectories)
                    .Select(Path.GetFileNameWithoutExtension);
        }
    }
}