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

        public CodeScanner()
        {
            _codeRepository = new CodeRepository();
            _storedProceduresRepository = new StoredProceduresRepository();
        }

        public CodeScanner(ICodeRepository codeRepository, 
            IStoredProceduresRepository storedProceduresRepository)
        {
            _codeRepository = codeRepository;
            _storedProceduresRepository = storedProceduresRepository;
        }

        public IEnumerable<string> GetUnusedStoredProcedures(string codeRootPath, string storedProcedureRootPath,
            int threads = DefaultThreadsCount, string[] excludedFileTypes = null, string[] excludedDirectories = null)
        {
            if (threads == DefaultThreadsCount)
                threads = Environment.ProcessorCount;

            IEnumerable<string> storedProcedures =
                _storedProceduresRepository.GetStoreProceduresNames(storedProcedureRootPath).ToList();
            IEnumerable<ScanResult> scanResults = ScanCode(codeRootPath, storedProcedures, threads, excludedFileTypes, excludedDirectories);

            return FilterUnusedStoredProcedures(storedProcedures, scanResults);
        }

        private IEnumerable<ScanResult> ScanCode(string codeRootPath, IEnumerable<string> storedProcedures, 
            int threads, string[] excludedFileTypes = null, string[] excludedDirectories = null)
        {
            var resultQueue = new ConcurrentQueue<ScanResult>();
            IEnumerable<string> filesList = 
                _codeRepository.GetCodeFilesPaths(codeRootPath, excludedFileTypes, excludedDirectories);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = threads
            };

            Parallel.ForEach(filesList, parallelOptions, x => ScanFile(x, storedProcedures, resultQueue));
            return resultQueue.ToArray();
        }

        private IEnumerable<string> FilterUnusedStoredProcedures(IEnumerable<string> storedProcedures, IEnumerable<ScanResult> scanResults)
        {
            Dictionary<string, int> results = storedProcedures.ToDictionary(s => s, s => 0);

            foreach (ScanResult scanResult in scanResults)
            {
                results[scanResult.SearchPattern]++;
            }

            return results.Where(x => x.Value == 0).Select(x => x.Key);
        }

        private void ScanFile(string filePath, IEnumerable<string> searchPatterns, ConcurrentQueue<ScanResult> scanResults)
        {
            var patterns = searchPatterns as IList<string> ?? searchPatterns.ToList();

            var results = _codeRepository.SearchFile(filePath, patterns);

            foreach (var scanResult in results)
            {
                scanResults.Enqueue(scanResult);
            } 
        }
    }
}
