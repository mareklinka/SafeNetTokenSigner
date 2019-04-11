using System;
using System.Runtime.InteropServices;

namespace SafenetSign.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIGNER_SUBJECT_INFO
    {
        /// DWORD->unsigned int
        public uint cbSize;

        /// DWORD*
        public IntPtr pdwIndex;

        /// DWORD->unsigned int
        public uint dwSubjectChoice;

        /// SubjectChoiceUnion
        public SubjectChoiceUnion SubjectChoice;

        [StructLayout(LayoutKind.Explicit)]
        public struct SubjectChoiceUnion
        {
            /// SIGNER_FILE_INFO*
            [FieldOffset(0)]
            public IntPtr pSignerFileInfo;

            /// SIGNER_BLOB_INFO*
            [FieldOffset(0)]
            public IntPtr pSignerBlobInfo;
        }
    }
}