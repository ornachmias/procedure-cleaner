using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using PC.Common;

namespace PC.DataAccess
{
    public class CodeRepository : ICodeRepository
    {
        public IEnumerable<string> GetCodeFilesPaths(string rootPath, string[] excludedFileTypes = null, string[] excludedDirectories = null)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException(rootPath);

            string[] files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);

            if (excludedFileTypes != null)
                files = files.Where(x => excludedFileTypes.All(y => !x.ToLower().EndsWith(y.ToLower()))).ToArray();

            if (excludedDirectories != null)
                files = files.Where(x => excludedDirectories.All(y=> !x.ToLower().Contains(y.ToLower()))).ToArray();

            files = RemoveDuplicateFiles(files);

            return files;
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
                            LineNumber = i,
                            FilePath = path
                        };

                        results.Add(scanResult);
                    }
                }
            }

            return results;
        }

        private string[] RemoveDuplicateFiles(string[] filePaths)
        {
            var filesHash = new HashSet<string>();
            var result = new List<string>();

            foreach (var filePath in filePaths)
            {
                var hash = CalculateFileHash(filePath);

                if (filesHash.Contains(hash))
                    continue;

                filesHash.Add(hash);
                result.Add(filePath);
            }

            return result.ToArray();
        }

        private string CalculateFileHash(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}