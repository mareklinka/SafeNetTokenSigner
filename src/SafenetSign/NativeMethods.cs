using System;
using System.Runtime.InteropServices;
using SafenetSign.Native;

namespace SafenetSign
{
    public static class NativeMethods
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptAcquireContext(
            ref IntPtr hProv, 
            string pszContainer,
            string pszProvider, 
            uint dwProvType, 
            uint dwFlags);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptSetProvParam(
            IntPtr hProv,
            uint dwParam,
            [In] byte[] pbData,
            uint dwFlags);

        [DllImport("CRYPT32.DLL", EntryPoint = "CertOpenStore", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CertOpenStore(
            IntPtr lpszStoreProvider,
            uint dwMsgAndCertEncodingType,
            IntPtr hCryptProv,
            uint dwFlags,
            string pvPara);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertFindCertificateInStore(
            IntPtr hCertStore,
            uint dwCertEncodingType,
            uint dwFindFlags,
            uint dwFindType,
            IntPtr pszFindPara, 
            IntPtr pPrevCertCntxt);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(
            string lpFileName, 
            IntPtr hReservedNull,
            LoadLibraryFlags dwFlags);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName);

        public delegate int SignerSignEx2Delegate(uint dwFlags, IntPtr pSubjectInfo, IntPtr pSignerCert, IntPtr pSignatureInfo,
            IntPtr pProviderInfo, uint dwTimestampFlags, IntPtr pszTimestampAlgorithmOid, IntPtr pwszHttpTimeStamp,
            IntPtr psRequest, IntPtr pSipData, IntPtr ppSignerContext, IntPtr pCryptoPolicy, IntPtr pReserved);
    }
}