using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIGNER_SIGNATURE_INFO
    {
        public uint cbSize;
        public uint algidHash; // ALG_ID
        public uint dwAttrChoice;
        public IntPtr pAttrAuthCode;
        public IntPtr psAuthenticated; // PCRYPT_ATTRIBUTES
        public IntPtr psUnauthenticated; // PCRYPT_ATTRIBUTES
    }
}