using System.Reflection;
using System.Runtime.InteropServices;

// This should be the same version as below
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if DEBUG
[assembly: AssemblyInformationalVersion("1.0.0-PreRelease")]
#else
[assembly: AssemblyInformationalVersion("1.0.0")]
#endif

[assembly: AssemblyCompany("Vapolia")]
[assembly: AssemblyProduct("XamarinFormsGesture")]
[assembly: AssemblyCopyright("Copyright ©2017 Vapolia")]
[assembly: AssemblyTrademark("Vapolia")]
[assembly: ComVisible(false)]
