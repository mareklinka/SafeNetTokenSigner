#include <stdio.h>
#include <windows.h>
#include <Wincrypt.h>
#include <iostream>
#include <string>
#include <cryptuiapi.h>
#include <tchar.h>
#include "Signer.h"
#pragma comment (lib, "cryptui.lib")
#pragma comment(lib, "crypt32.lib")

// this code was adapted from
// https://stackoverflow.com/questions/48804073/signing-an-appxbundle-using-cryptuiwizdigitalsign-api
// https://msdn.microsoft.com/en-us/library/windows/desktop/jj835834(v=vs.85).aspx

int wmain(int argc, wchar_t** argv)
{
	if (argc < 7)
	{
		std::wcout << L"usage: signer.exe <certificate thumbprint> <private key container name> <token PIN> <timestamp URL> <mode> <path to file to sign>\n";
		std::wcout << L"mode = (appx|pe)\n";
		return 1;
	}

	const std::wstring certHash = argv[1];
	const std::wstring containerName = argv[2];
	const std::wstring tokenPin = argv[3];
	const std::wstring timestampUrl = argv[4];
	const std::wstring mode = argv[5];
	const std::wstring fileToSign = argv[6];

	if (mode.compare(L"appx") != 0 && mode.compare(L"pe") != 0)
	{
		std::wcout << L"usage: signer.exe <certificate thumbprint> <private key container name> <token PIN> <timestamp URL> <mode> <path to file to sign>\n";
		std::wcout << L"mode = (appx|pe)\n";
		return 1;
	}

	std::wcout << L"Certificate thumbprint: " << certHash << L"\n";
	std::wcout << L"Timestamp URL: " << timestampUrl << L"\n";
	std::wcout << L"File to sign: " << fileToSign << L"\n";

	BYTE* pHashData = new byte[20];
	DWORD cbBinary = 20, dwSkip = 0, dwFlags = 0;

	if (!CryptStringToBinary(certHash.c_str(), 40, CRYPT_STRING_HEXRAW, pHashData, &cbBinary, &dwSkip, &dwFlags))
	{
		std::wcout << L"Converting the certificate thumbprint to byte array failed. Check the provided SHA1 data." << L"\n";
		exit(1);
	}

	std::wcout << L"Logging into the token... ";
	HCRYPTPROV cryptoContext = UnlockToken(containerName, utf16_to_utf8(tokenPin));

	if (cryptoContext)
	{
		std::wcout << L"Success" << L"\n";
	}
	else
	{
		std::wcout << L"Failure!" << L"\n";
		exit(1);
	}

	HCERTSTORE  hSystemStore;
	PCCERT_CONTEXT  pDesiredCert = NULL;
	PCCERT_CONTEXT  pCertContext;

	if (hSystemStore = CertOpenStore(
		CERT_STORE_PROV_SYSTEM_W,
		0,
		NULL,
		CERT_SYSTEM_STORE_CURRENT_USER,
		L"MY"))
	{
		std::wcout << L"Certificate store opened successfully" << L"\n";
	}
	else
	{
		std::wcerr << L"Unable to open certificate store, error " << std::hex << std::showbase << ::GetLastError() << L"\n";
		exit(1);
	}

	CRYPT_HASH_BLOB hashBlob = {};
	hashBlob.cbData = 20;
	hashBlob.pbData = pHashData;

	if (pDesiredCert = CertFindCertificateInStore(
		hSystemStore,
		MY_ENCODING_TYPE,
		0,
		CERT_FIND_SHA1_HASH,
		&hashBlob,
		NULL))
	{
		std::wcout << L"Certificate found" << L"\n";
	}
	else
	{
		std::wcerr << L"Unable to find the requested certificate (" << certHash << "), error " << std::hex << std::showbase << ::GetLastError() << L"\n";
		exit(1);
	}

	std::wcout << L"Signing file " << fileToSign << "... ";

	HRESULT res = SignFile(pDesiredCert, fileToSign.c_str(), timestampUrl.c_str(), mode == L"appx");

	if (res == S_OK)
	{
		std::wcout << L"Success" << L"\n";
	}
	else
	{
		std::wcerr << L"Signing of the package failed in SignAppxPackage, error " << std::hex << std::showbase << ::GetLastError() << L"\n";
		exit(1);
	}

	std::wcout << L"Signing complete\n";

	if (hSystemStore)
	{
		CertCloseStore(
			hSystemStore,
			CERT_CLOSE_STORE_CHECK_FLAG);
	}

	if (pDesiredCert)
	{
		CertFreeCertificateContext(pDesiredCert);
	}

	if (cryptoContext)
	{
		CryptReleaseContext(cryptoContext, 0);
	}
}

HCRYPTPROV UnlockToken(const std::wstring& containerName, const std::string& tokenPin)
{
	CryptProvHandle cryptProv;
	if (!::CryptAcquireContext(&cryptProv.Handle, containerName.c_str(), ETOKEN_BASE_CRYPT_PROV_NAME.c_str(), PROV_RSA_FULL, CRYPT_SILENT))
	{
		std::wcerr << L"CryptAcquireContext failed, error " << std::hex << std::showbase << ::GetLastError() << L"\n";
		return NULL;
	}

	if (!::CryptSetProvParam(cryptProv.Handle, PP_SIGNATURE_PIN, reinterpret_cast<const BYTE*>(tokenPin.c_str()), 0))
	{
		std::wcerr << L"CryptSetProvParam failed, error " << std::hex << std::showbase << ::GetLastError() << L"\n";
		return NULL;
	}

	auto result = cryptProv.Handle;
	cryptProv.Handle = NULL;
	return result;
}

HRESULT SignFile(
	_In_ PCCERT_CONTEXT signingCertContext,
	_In_ LPCWSTR packageFilePath,
	_In_ PCWSTR timestampUrl,
	_In_ BOOL isSigningAppx)
{
	HRESULT hr = S_OK;

	// Initialize the parameters for SignerSignEx2
	DWORD signerIndex = 0;

	SIGNER_FILE_INFO fileInfo = {};
	fileInfo.cbSize = sizeof(SIGNER_FILE_INFO);
	fileInfo.pwszFileName = packageFilePath;

	SIGNER_SUBJECT_INFO subjectInfo = {};
	subjectInfo.cbSize = sizeof(SIGNER_SUBJECT_INFO);
	subjectInfo.pdwIndex = &signerIndex;
	subjectInfo.dwSubjectChoice = SIGNER_SUBJECT_FILE;
	subjectInfo.pSignerFileInfo = &fileInfo;

	SIGNER_CERT_STORE_INFO certStoreInfo = {};
	certStoreInfo.cbSize = sizeof(SIGNER_CERT_STORE_INFO);
	certStoreInfo.dwCertPolicy = SIGNER_CERT_POLICY_CHAIN_NO_ROOT;
	certStoreInfo.pSigningCert = signingCertContext;

	SIGNER_CERT cert = {};
	cert.cbSize = sizeof(SIGNER_CERT);
	cert.dwCertChoice = SIGNER_CERT_STORE;
	cert.pCertStoreInfo = &certStoreInfo;

	SIGNER_SIGNATURE_INFO signatureInfo = {};
	signatureInfo.cbSize = sizeof(SIGNER_SIGNATURE_INFO);
	signatureInfo.algidHash = CALG_SHA_256;
	signatureInfo.dwAttrChoice = SIGNER_NO_ATTR;

	SIGNER_SIGN_EX2_PARAMS signerParams = {};
	signerParams.pSubjectInfo = &subjectInfo;
	signerParams.pSigningCert = &cert;
	signerParams.pSignatureInfo = &signatureInfo;
	signerParams.dwTimestampFlags = SIGNER_TIMESTAMP_AUTHENTICODE;
	signerParams.pwszTimestampURL = timestampUrl;

	APPX_SIP_CLIENT_DATA sipClientData;
	if (isSigningAppx)
	{
		// we only use this when signing appx packages
		sipClientData = {};
		sipClientData.pSignerParams = &signerParams;
		signerParams.pSipData = &sipClientData;
	}

	// Type definition for invoking SignerSignEx2 via GetProcAddress
	typedef HRESULT(WINAPI *SignerSignEx2Function)(
		DWORD,
		PSIGNER_SUBJECT_INFO,
		PSIGNER_CERT,
		PSIGNER_SIGNATURE_INFO,
		PSIGNER_PROVIDER_INFO,
		DWORD,
		PCSTR,
		PCWSTR,
		PCRYPT_ATTRIBUTES,
		PVOID,
		PSIGNER_CONTEXT *,
		PVOID,
		PVOID);

	// Load the SignerSignEx2 function from MSSign32.dll
	HMODULE msSignModule = LoadLibraryEx(
		L"MSSign32.dll",
		NULL,
		LOAD_LIBRARY_SEARCH_SYSTEM32);

	if (msSignModule)
	{
		SignerSignEx2Function SignerSignEx2 = reinterpret_cast<SignerSignEx2Function>(
			GetProcAddress(msSignModule, "SignerSignEx2"));
		if (SignerSignEx2)
		{
			hr = SignerSignEx2(
				signerParams.dwFlags,
				signerParams.pSubjectInfo,
				signerParams.pSigningCert,
				signerParams.pSignatureInfo,
				signerParams.pProviderInfo,
				signerParams.dwTimestampFlags,
				signerParams.pszAlgorithmOid,
				signerParams.pwszTimestampURL,
				signerParams.pCryptAttrs,
				signerParams.pSipData,
				signerParams.pSignerContext,
				signerParams.pCryptoPolicy,
				signerParams.pReserved);
		}
		else
		{
			DWORD lastError = GetLastError();
			hr = HRESULT_FROM_WIN32(lastError);
		}

		FreeLibrary(msSignModule);
	}
	else
	{
		DWORD lastError = GetLastError();
		hr = HRESULT_FROM_WIN32(lastError);
	}

	if (isSigningAppx && sipClientData.pAppxSipState)
	{
		sipClientData.pAppxSipState->Release();
	}

	return hr;
}

std::string utf16_to_utf8(const std::wstring& str)
{
	if (str.empty())
	{
		return "";
	}

	auto utf8len = ::WideCharToMultiByte(CP_UTF8, 0, str.data(), str.size(), NULL, 0, NULL, NULL);
	if (utf8len == 0)
	{
		return "";
	}

	std::string utf8Str;
	utf8Str.resize(utf8len);
	::WideCharToMultiByte(CP_UTF8, 0, str.data(), str.size(), &utf8Str[0], utf8Str.size(), NULL, NULL);

	return utf8Str;
}