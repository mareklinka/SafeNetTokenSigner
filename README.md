# Update March 16, 2020

This project is no longer maintained. Instead, you can use a sign-tool compatible approach detailed in [this issue](https://github.com/mareklinka/SafeNetTokenSigner/issues/8).

----

[![Build status](https://dev.azure.com/mar3ek/safenet-signer/_apis/build/status/safenet-signer-CI)](https://dev.azure.com/mar3ek/safenet-signer/_build/latest?definitionId=13)

# SafeNetTokenSigner
A SignTool alternative that can unlock SafeNet hardware tokens without user interaction.

## What it does
Signing your released code is an important step in ensuring integrity of the code and trust of your users. Unfortunately, some code signing certificates come to you on hardware tokens protected by a PIN and so it's very difficult (or impossible) to use them in any kind of automated manner (such as CI pipelines).

This tools attempts to solve this particular problem by allowing you to code-sign your binaries without user interaction even when the certificate requires a PIN to be unlocked. All this is done with standard Windows APIs and distributed as a dotnet global tool so it should work as long as you have the dotnet CLI installed.

## How it works
[Details available in Wiki](https://github.com/mareklinka/SafeNetTokenSigner/wiki/History-of-the-project-and-implementation-details)

## Usage
To make use of this tool, you either need to install it as a dotnet tool or compile it from source yourself. This tool is __x64 WINDOWS ONLY__ as it uses several Win32 API functions to delegate the actual signing to the OS.

### Installing the tool via [NuGet](https://www.nuget.org/packages/SafenetSign)
On the machine where you'll be doing your code signing:
1. Make sure you have .NET Core runtime installed on the machine (you can get the installer [here](https://dotnet.microsoft.com/download))
2. Run `dotnet tool install --global SafenetSign --version [specific version]`
   * The version parameter is currently required as the tool is marked as pre-release
   * Get the latest version number on [NuGet.org](https://www.nuget.org/packages/SafenetSign)
   * You don't necessarilly need to install with `--global` but then the tools won't be available machine wide
   * If you are unfamiliar with dotnet tools, you can read the [docs](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
3. Run `safenet-sign`
   * If you installed without `--global`, you will need to navigate to your installation path first
   * The tool should display a command line parameter help, confirming that it works
   * You *might* need to restart your command line for the changes to take effect

### Compiling from source
1. You'll need [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download)
2. Clone the repository
3. Navigate to `src\SafenetSign`
4. Run `dotnet publish .\SafenetSignNoTool.csproj -c Release --self-contained false`
   * or you can use Visual Studio or VS Code to open the solution and look around
5. Put the output (in `src\SafenetSign\bin\Release\netcoreapp2.2\win-x64\publish`) wherever you need it
6. Run `safenetsign.exe` to get the command line parameter help

### Comand line options
The tool has several required positional arguments and two optional arguments. You can get the full help by running the tool without any parameters (`safenet-sign`).

Required parameters:

* the signing certificate thumbprint (SHA1, e.g. `8cfaa82e5c3842ffee22d82d8ff812b3a1de1ebb`)
* private key container name - for SafeNet tokens, you can get this by looking into the SafeNet app (see this [SO question](https://stackoverflow.com/a/47894907/1453109))
* the target store that contains the certificate (valid values are `user` and `machine`)
* the PIN used to protect the token's private keys
* timestamp URL (the code will perform timestamping automatically, e.g. `http://timestamp.verisign.com/scripts/timstamp.dll`)
* mode is either `pe` or `appx`, depending on what file you are trying to sign (`pe` for normal EXE/DLL etc., `appx` for UWP app bundles)
* the path to the file you want to sign (can be relative)

Optional parameters:

* `-v` - switch, enables verbose activity logging for debugging or when you want to report an issue with the tool
* `-m [path]` - the tools makes use of Win32 APIs located in `MSSign32.dll`. This option allows you to specify a path to the library (useful for non-standard installations and similar but should otherwise not be required)

Schema:

`<executable> <cert thumbprint> <private key container name> <target store> <token PIN> <timestamp URL> <mode> <path to file to sign> [-v] [-m path]`

Examples:

.NET Core - dotnet tool

`safenet-sign 8cfaa82e5c3842ffee22d82d8ff812b3a1de1ebb my-container-name 12345678 user http://timestamp.verisign.com/scripts/timstamp.dll pe Thing.exe -v -m "C:\MSSign32.dll"`

.NET Core - binaries or self-compiled

`safenetsign.exe 8cfaa82e5c3842ffee22d82d8ff812b3a1de1ebb my-container-name 12345678 user http://timestamp.verisign.com/scripts/timstamp.dll pe Thing.exe -v -m "C:\MSSign32.dll"`

## Contributing
I wrote the original code to solve a very specific problem I faced in my day-to-day work. Over time, it gathered a small amount of interest so I decided to rewrite the whole thing and make it a little bit more official (and flexible AND user friendly).

I'm open to contributions, be it a PR or an issue and I'll do my best to answer you requests and questions.

## C++ version
You can also find the precursor of this tool in this repo, written entirely in C++ (VS 2017 or 2019 with C++ workload required to compile). __This version is no longer maintained and does not support all the options of the .NET version__.

## Limitations
This code is provided without any guuarantees. We use it as part of our production-grade CI pipeline (self-hosted Azure DevOps build agent on a physical Windows 10 machine) and it performs as expected. Your milage may vary but if you give the tool a try and find an issue, please report it and I'll do my best to fix it.

## Thanks where it's due
It was only possible to create this tool because of SO users [draketb](https://stackoverflow.com/users/1751253/draketb) and [RbMm](https://stackoverflow.com/users/6401656/rbmm) (and others). So big thanks to them and the whole SO community!
