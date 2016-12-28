using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        private readonly ConcurrentQueue<string> _filesQueue;
        private readonly ConcurrentQueue<ScanResult> _resultQueue;

        public CodeScanner(ICodeRepository codeRepository, 
            IStoredProceduresRepository storedProceduresRepository)
        {
            _codeRepository = codeRepository;
            _storedProceduresRepository = storedProceduresRepository;
            _filesQueue = new ConcurrentQueue<string>();
            _resultQueue = new ConcurrentQueue<ScanResult>();
        }

        public void ScanCode(string codeRootPath, string storedProcedureRootPath, int threads = DefaultThreadsCount)
        {
            foreach (var filesPath in _codeRepository.GetCodeFilesPaths(codeRootPath))
            {
                _filesQueue.Enqueue(filesPath);
            }

            var storeProcedures = _storedProceduresRepository.GetStoreProceduresNames(storedProcedureRootPath);

            if (threads == DefaultThreadsCount)
                threads = Environment.ProcessorCount;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = threads
            };
            
            Parallel.Invoke(parallelOptions, () => ScanFile(storeProcedures));
        }

        private void ScanFile(IEnumerable<string> searchPatterns)
        {
            string filePath;

            while (!_filesQueue.TryDequeue(out filePath)) { }

            var results = _codeRepository.SearchFile(filePath, searchPatterns);

            foreach (var scanResult in results)
            {
                _resultQueue.Enqueue(scanResult);
            }
        }
    }
}
