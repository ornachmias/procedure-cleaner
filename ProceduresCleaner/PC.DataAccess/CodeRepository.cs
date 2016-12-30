using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PC.Common;

namespace PC.DataAccess
{
    public class CodeRepository : ICodeRepository
    {
        public IEnumerable<string> GetCodeFilesPaths(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException(rootPath);

            return Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
        }

        public IEnumerable<ScanResult> SearchFile(string path, IEnumerable<string> searchPatterns)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            string[] lines = File.ReadAllLines(path);
            var patterns = searchPatterns as IList<string> ?? searchPatterns.ToList();
            
            var results = new List<ScanResult>();

            for (var i = 0; i < lines.Length; i++)
            {
                foreach (var searchPattern in patterns)
                {
                    if (searchPattern.ToLower().Equals(Path.GetFileNameWithoutExtension(path)))
                        continue;

                    if (lines[i].ToLower().Contains(searchPattern.ToLower()))
                    {
                        var scanResult = new ScanResult
                        {
                            Id = Guid.NewGuid().ToString(),
                            Line = lines[i],
                            SearchPattern = searchPattern,
                            LineNumber = i
                        };

                        results.Add(scanResult);
                    }
                }
            }

            return results;
        }
    }
}