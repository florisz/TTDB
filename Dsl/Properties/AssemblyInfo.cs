#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"Luminis")]
[assembly: AssemblyProduct(@"Its")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"Luminis.Its.Workbench.DslPackage, PublicKey=00240000048000009400000006020000002400005253413100040000010001005538BDD1151AE4891AEA6C6A2E310979C339554A212BBEAF3E9A8618C0C396561886D957757BD4BA84BDF505687A57C35632118210B80D3E53984F5B946AEB3AF6E087AC5C7760CF2CB695FDE1A277A4A0FB8D0426364D32C6E4E358784748A8321948F5631DC6F23435D5C64AD4A7EC5FEE394DB9A235F4CD541BB2D5551A8C")]