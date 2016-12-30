using NDesk.Options;
using PC.Scanner;

namespace PC.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = ParseArguments(args);
            var codeScanner = new CodeScanner();
            var unusedProcedures = 
                codeScanner.GetUnusedStoredProcedures(parameters.CodePath, parameters.StoredProceduresPath);

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
                    v => showHelp = v != null
                }
            };

            try
            {
                if (!showHelp)
                {
                    PrintHelp();
                }
                else
                {
                    options.Parse(args);
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

