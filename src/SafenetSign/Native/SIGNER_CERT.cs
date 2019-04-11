using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIGNER_CERT
    {
        public uint cbSize;

        public uint dwCertChoice;

        public SignerCertSourceUnion SignerCertSource;

        [StructLayout(LayoutKind.Explicit)]
        public struct SignerCertSourceUnion
        {
            [FieldOffset(0)]
            public IntPtr pwszSpcFile;

            [FieldOffset(0)]
            public IntPtr pCertStoreInfo;

            [FieldOffset(0)]
            public IntPtr pSpcChainInfo;
        }

        public IntPtr hwnd;
    }
}