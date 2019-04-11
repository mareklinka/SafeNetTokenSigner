using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIGNER_CERT_STORE_INFO
    {
        public uint cbSize;

        public IntPtr pSigningCert;

        public uint dwCertPolicy;

        public IntPtr hCertStore;
    }
}