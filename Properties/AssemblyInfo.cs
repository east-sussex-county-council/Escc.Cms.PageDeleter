using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Escc.Cms.PageDeleter")]
[assembly: AssemblyDescription("Deletes pages in a given channel which are older than a given threshold")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("East Sussex County Council")]
[assembly: AssemblyProduct("Escc.Cms.PageDeleter")]
[assembly: AssemblyCopyright("Copyright © East Sussex County Council 2007")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4c8f6f94-b7f0-4ccd-80ee-ebcd6588d993")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Recommended by FxCop
[assembly: CLSCompliant(true)]

// Can't apply CAS limitations because it's dependent on assemblies (Enterprise Library) which require full trust.
[assembly: NeutralResourcesLanguageAttribute("en-GB")]


// Use log4net web.config settings
[assembly: log4net.Config.XmlConfigurator]