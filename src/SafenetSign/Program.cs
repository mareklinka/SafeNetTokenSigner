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

            if (args.Length != 7)
            {
                Console.WriteLine("usage: signer.exe <certificate thumbprint> <private key container name> <target store> <token PIN> <timestamp URL> <storeString> <path to file to sign>");
                Console.WriteLine("storeString = (appx|pe)");
                Console.WriteLine("target store = (user|machine)");

                return 1;
            }

            int index = 0;

            var certHash = args[index++];
            var containerName = args[index++];
            var targetStore = args[index++];
            var tokenPin = args[index++];
            var timestampUrl = args[index++];
            var mode = args[index++];
            var fileToSign = args[index++];

            try
            {
                var signMode = ParseMode(mode);
                var store = ParseStore(targetStore);

                CodeSigner.SignFile(certHash, tokenPin, containerName, store, fileToSign, timestampUrl,
                    signMode, Console.WriteLine);

                return 0;
            }
            catch (SigningException ex)
            {
                Console.Error.WriteLine("Signing operation failed. Error details:");
                Console.Error.WriteLine(ex.GetBaseException().Message);

                return 2;
            }
        }

        private static CertificateStore ParseStore(string storeString)
        {
            CertificateStore store;
            switch (storeString.ToLowerInvariant())
            {
                case "user":
                    store = CertificateStore.User;
                    break;
                case "machine":
                    store = CertificateStore.Machine;
                    break;
                default:
                    throw new SigningException($"Unknown store specified: {storeString}");
            }

            return store;
        }

        private static SignMode ParseMode(string mode)
        {
            SignMode signMode;
            switch (mode.ToLowerInvariant())
            {
                case "pe":
                    signMode = SignMode.PE;
                    break;
                case "appx":
                    signMode = SignMode.APPX;
                    break;
                default:
                    throw new SigningException($"Unknown storeString specified: {mode}");
            }

            return signMode;
        }
    }
}
