using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Args;

namespace SafenetSign
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.OSArchitecture != Architecture.X64)
            {
                Console.Error.WriteLine("This tool only supports x64 Windows");
                return 10;
            }

            var argumentModel = Configuration.Configure<CommandParameters>();

            CommandParameters command;
            try
            {
                command = argumentModel.CreateAndBind(args);

                if (!string.IsNullOrEmpty(command.MsSign32Path) && !Path.IsPathRooted(command.MsSign32Path))
                {
                    // https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibraryexa#searching-for-dlls-and-dependencies
                    Console.Error.WriteLine("Specifying a relative path for the MSSign32.dll is not supported");
                    return 9;
                }
            }
            catch (Exception)
            {
                PrintHelp(argumentModel);

                return 1;
            }

            try
            {
                CodeSigner.SignFile(command.Thumbprint, command.Pin, command.PrivateKeyContainer, command.Store,
                    command.Path, command.TimestampUrl,
                    command.Mode, command.MsSign32Path, new Logger(command.IsVerboseLoggingEnabled));

                return 0;
            }
            catch (SigningException ex)
            {
                Console.Error.WriteLine("Signing operation failed. Error details:");
                Console.Error.WriteLine(ex.GetBaseException().Message);

                return 2;
            }
        }

        private static void PrintHelp(IModelBindingDefinition<CommandParameters> argumentModel)
        {
            Console.WriteLine("Invalid command line arguments specified.");
            Console.WriteLine();

            var arguments = string.Join(" ", argumentModel.Members.Select(_ => $"<{_.MemberInfo.Name}>"));
            Console.WriteLine($"Usage: {arguments}");
            Console.WriteLine();

            var list = new List<string[]> { new[] { "Argument", "Description", "Is required", "Default value" } };

            foreach (var member in argumentModel.Members)
            {
                list.Add(new[]
                {
                    member.MemberInfo.Name, member.HelpText, member.Required.ToString(),
                    member.DefaultValue?.ToString() ?? string.Empty
                });
            }

            var lengths = Enumerable.Range(0, list[0].Length).Select(_ => list.Max(a => a[_].Length)).ToList();

            var rowWidth = lengths.Sum() + list[0].Length * 3 + 1;

            PrintRowSeparator(rowWidth);
            PrintHelpRow(list[0], lengths);
            PrintRowSeparator(rowWidth);

            foreach (var row in list.Skip(1))
            {
                PrintHelpRow(row, lengths);
            }

            PrintRowSeparator(rowWidth);
        }

        private static void PrintRowSeparator(int rowWidth)
        {
            Console.WriteLine(new string('-', rowWidth));
        }

        private static void PrintHelpRow(string[] row, List<int> lengths)
        {
            for (var index = 0; index < row.Length; index++)
            {
                var s = row[index];
                Console.Write($"| {s.PadRight(lengths[index])} ");
            }

            Console.WriteLine("|");
        }
    }
}
