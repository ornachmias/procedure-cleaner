using System.Collections.Generic;

namespace PC.DataAccess
{
    public interface ICodeRepository
    {
        IEnumerable<string> GetCodeFilesPaths(string rootDir);
    }
}
