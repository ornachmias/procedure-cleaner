using System.Configuration;
using System.Linq;
using NDesk.Options;
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
                ConfigurationManager.AppSettings["ExcludedFileExtensions"].Split(';').Select(x => x.ToLower()).ToArray();
            if (!excludedFileTypes.Any())
                excludedFileTypes = null;

            var excludedFolderPaths = 
                ConfigurationManager.AppSettings["ExcludedFolderPaths"].Split(';').Select(x => x.ToLower()).ToArray();
            if (!excludedFolderPaths.Any())
                excludedFolderPaths = null;

            var codeScanner = new CodeScanner();
            var unusedProcedures =
                codeScanner.GetUnusedStoredProcedures(parameters.CodePath, parameters.StoredProceduresPath, -1,
                    excludedFileTypes, excludedFolderPaths);

            foreach (var procedure in unusedProcedures)
            {
                System.Console.WriteLine(procedure);
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
        }

        private class UserParameters
        {
            public string StoredProceduresPath { get; set; }
            public string CodePath { get; set; }
        }
    }
}

