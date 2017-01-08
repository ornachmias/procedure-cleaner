using System.Configuration;
using System.Linq;
using NDesk.Options;
using PC.Common;
using PC.Scanner;

namespace PC.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = ParseArguments(args);

            if (parameters == null)
                return;

            var excludedFileTypes =
                ConfigurationManager.AppSettings["ExcludedFileExtensions"]
                .Split(';').Select(x => x.ToLower().Trim()).ToArray();

            if (!excludedFileTypes.Any())
                excludedFileTypes = null;

            var excludedFolderPaths = 
                ConfigurationManager.AppSettings["ExcludedFolderPaths"]
                .Split(';').Select(x => x.ToLower().Trim()).ToArray();

            excludedFolderPaths = excludedFolderPaths.Concat(parameters.ExcludedDirectories).ToArray();
            
            if (!excludedFolderPaths.Any())
                excludedFolderPaths = null;

            var codeScanner = new CodeScanner();

            if (string.IsNullOrEmpty(parameters.ProceduresIndication))
            {
                var unusedProcedures =
                codeScanner.GetUnusedStoredProcedures(parameters.CodePath, parameters.StoredProceduresPath,
                    excludedFileTypes, excludedFolderPaths);

                foreach (var procedure in unusedProcedures)
                {
                    System.Console.WriteLine(procedure);
                }
            }
            else
            {
                var scanResults = codeScanner.GetNotImplementedProcedures(parameters.ProceduresIndication,
                    parameters.CodePath, parameters.StoredProceduresPath, excludedFileTypes, excludedFolderPaths);

                foreach (var result in scanResults)
                {
                    PrintScanResult(result);
                    System.Console.WriteLine();
                }
            }
        }

        private static UserParameters ParseArguments(string[] args)
        {
            var parameters = new UserParameters();
            bool showHelp = false;

            var options = new OptionSet
            {
                {
                    "c|code=", 
                    v => parameters.CodePath = v
                },
                {
                    "s|sp=",
                    v => parameters.StoredProceduresPath = v
                },
                {
                    "e|ex=",
                    v => parameters.ExcludedDirectories = v.Split(',').Select(x=>x.Trim()).ToArray()
                },
                {
                    "i|implement=",
                    v => parameters.ProceduresIndication = v.Trim()
                },
                {
                    "h|help",
                    v => showHelp = true
                }
            };

            try
            {
                options.Parse(args);

                if (showHelp)
                {
                    PrintHelp();
                    return null;
                }
            }
            catch (OptionException ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine();
                PrintHelp();
            }

            return parameters;
        }

        private static void PrintHelp()
        {
            System.Console.WriteLine("Required Parameters:");
            System.Console.WriteLine("s|sp");
            System.Console.WriteLine("Path to the Stored Procedures");
            System.Console.WriteLine();
            System.Console.WriteLine("c|code");
            System.Console.WriteLine("Path to the code files");
            System.Console.WriteLine();
            System.Console.WriteLine("e|exclude");
            System.Console.WriteLine("Partial directories paths separated by commas to exclude");
            System.Console.WriteLine();
            System.Console.WriteLine("i|implement");
            System.Console.WriteLine("Find not implemented stored procedures by string indication");
        }

        private static void PrintScanResult(ScanResult result)
        {
            System.Console.WriteLine(result.FilePath);
            System.Console.WriteLine(result.Line);
        }

        private class UserParameters
        {
            public string StoredProceduresPath { get; set; }
            public string CodePath { get; set; }
            public string[] ExcludedDirectories { get; set; }
            public string ProceduresIndication { get; set; }
        }
    }
}

