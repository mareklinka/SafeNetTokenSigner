using System;
using System.Runtime.InteropServices;

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

            if (args.Length != 6)
            {
                Console.WriteLine("usage: signer.exe <certificate thumbprint> <private key container name> <token PIN> <timestamp URL> <mode> <path to file to sign>");
                Console.WriteLine("mode = (appx|pe)");

                return 1;
            }

            var certHash = args[0];
            var containerName = args[1];
            var tokenPin = args[2];
            var timestampUrl = args[3];
            var mode = args[4];
            var fileToSign = args[5];

            try
            {
                CodeSigner.SignFile(certHash, tokenPin, containerName, fileToSign, timestampUrl,
                       mode.Equals("pe", StringComparison.OrdinalIgnoreCase) ? SignMode.PE : SignMode.APPX);

                return 0;
            }
            catch (SigningException ex)
            {
                Console.Error.WriteLine("Signing operation failed. Error details:");
                Console.Error.WriteLine(ex.GetBaseException().Message);

                return 2;
            }
        }
    }
}
