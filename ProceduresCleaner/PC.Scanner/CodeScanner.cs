using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PC.Common;
using PC.DataAccess;

namespace PC.Scanner
{
    public class CodeScanner
    {
        public const int DefaultThreadsCount = -1;
        private readonly ICodeRepository _codeRepository;
        private readonly IStoredProceduresRepository _storedProceduresRepository;
        private ConcurrentQueue<ScanResult> _resultQueue;

        public CodeScanner(ICodeRepository codeRepository, 
            IStoredProceduresRepository storedProceduresRepository)
        {
            _codeRepository = codeRepository;
            _storedProceduresRepository = storedProceduresRepository;
        }

        public void ScanCode(string codeRootPath, string storedProcedureRootPath,
            ConcurrentQueue<ScanResult> resultQueue, int threads = DefaultThreadsCount)
        {
            _resultQueue = resultQueue;
            IEnumerable<string> filesList = _codeRepository.GetCodeFilesPaths(codeRootPath);

            var storeProcedures = _storedProceduresRepository.GetStoreProceduresNames(storedProcedureRootPath);

            if (threads == DefaultThreadsCount)
                threads = Environment.ProcessorCount;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = threads
            };

            Parallel.ForEach(filesList, parallelOptions, x => ScanFile(x, storeProcedures));
        }

        private void ScanFile(string filePath, IEnumerable<string> searchPatterns)
        {
            var patterns = searchPatterns as IList<string> ?? searchPatterns.ToList();

            var results = _codeRepository.SearchFile(filePath, patterns);

            foreach (var scanResult in results)
            {
                _resultQueue.Enqueue(scanResult);
            } 
        }
    }
}
