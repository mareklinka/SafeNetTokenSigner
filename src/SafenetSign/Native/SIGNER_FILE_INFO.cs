using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIGNER_FILE_INFO
    {
        public uint cbSize;
        public IntPtr pwszFileName;
        public IntPtr hFile;
    }
}