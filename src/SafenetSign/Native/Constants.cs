namespace SafenetSign.Native
{
    public static class Constants
    {
        public const uint PROV_RSA_FULL = 1;
        public const int CRYPT_SILENT = 0x00000040;

        public const uint CERT_STORE_PROV_SYSTEM = 10;

        public const uint DONT_CARE = 0;

        public const uint CERT_SYSTEM_STORE_CURRENT_USER = 1 << 16;
        public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE = 2 << 16;

        public const int MY_ENCODING_TYPE = 0x00010000 | 0x00000001;
        public const int CERT_FIND_SHA1_HASH = 1 << 16;

        public const int SIGNER_SUBJECT_FILE = 0x01;
        public const int SIGNER_CERT_POLICY_CHAIN_NO_ROOT = 0x08;
        public const int SIGNER_CERT_STORE = 0x2;
        public const int CALG_SHA_256 = 0x0000800c;
        public const int SIGNER_TIMESTAMP_AUTHENTICODE = 1;

        public const int PP_SIGNATURE_PIN = 33;

        public const string CryptoProviderName = "eToken Base Cryptographic Provider";
        public const string CryptoStoreName = "MY";
    }
}
