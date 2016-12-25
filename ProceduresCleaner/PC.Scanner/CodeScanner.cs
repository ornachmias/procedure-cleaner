using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public IEnumerable<ScanResult> ScanCode(string rootDir, int threads = DefaultThreadsCount)
        {
            foreach (var filesPath in _codeRepository.GetCodeFilesPaths(rootDir))
            {
                _filesQueue.Enqueue(filesPath);
            }
            
            return null;
        }

        private void ScanFile(string filePath, List<string> searchPatterns)
        {
            if (!File.Exists(filePath))
                return;
            
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (var searchPattern in searchPatterns)
                {
                    if (lines[i].ToLower().Contains(searchPattern.ToLower()))
                    {
                        var scanResult = new ScanResult
                        {
                            Id = Guid.NewGuid().ToString(),
                            Line = lines[i],
                            SearchPattern = searchPattern,
                            LineNumber = i
                        };

                        _resultQueue.Enqueue(scanResult);
                    }
                }
            }
        }
    }
}
