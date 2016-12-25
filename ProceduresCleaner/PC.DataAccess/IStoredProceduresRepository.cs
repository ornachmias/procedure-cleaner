using System.Collections.Generic;

namespace PC.DataAccess
{
    public interface IStoredProceduresRepository
    {
        IEnumerable<string> GetStoreProceduresNames();
    }
}
