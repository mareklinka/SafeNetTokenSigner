using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CRYPTOAPI_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }
}
