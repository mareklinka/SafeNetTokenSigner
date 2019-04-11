using System;

namespace SafenetSign.Native
{
    public struct SIGNER_SIGN_EX2_PARAMS
    {
        public uint dwFlags;
        public IntPtr pSubjectInfo;
        public IntPtr pSigningCert;
        public IntPtr pSignatureInfo;
        public IntPtr pProviderInfo;
        public uint dwTimestampFlags;
        public IntPtr pszAlgorithmOid;
        public IntPtr pwszTimestampURL;
        public IntPtr pCryptAttrs;
        public IntPtr pSipData;
        public IntPtr pSignerContext;
        public IntPtr pCryptoPolicy;
        public IntPtr pReserved;
    }
}