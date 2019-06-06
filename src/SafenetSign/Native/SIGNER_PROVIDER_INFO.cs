using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    public struct SIGNER_PROVIDER_INFO
    {
        public uint cbSize;
        public IntPtr pwszProviderName;
        public uint dwProviderType;
        public uint dwKeySpec;
        public uint dwPvkChoice;
        public PvkChoiceUnion PvkChoice;

        [StructLayout(LayoutKind.Explicit)]
        public struct PvkChoiceUnion
        {
            [FieldOffset(0)]
            public IntPtr pwszPvkFileName;

            [FieldOffset(0)]
            public IntPtr pwszKeyContainer;
        }
    }
}