// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: AssemblyTitle("Microsoft.Samples.Data.Services.Client")]
[assembly: AssemblyDescription("Microsoft.Samples.Data.Services.Client")]
[assembly: AssemblyDefaultAlias("Microsoft.Samples.Data.Services.Client")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft\u00AE .NET Framework")]
[assembly: AssemblyCopyright("\u00A9 Microsoft Corporation.  All rights reserved.")]

[assembly: NeutralResourcesLanguageAttribute("en-US")]

[assembly: SecurityCritical]

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]

[assembly: AssemblyVersion(ThisAssembly.Version)]
[assembly: AssemblyFileVersion(ThisAssembly.InformationalVersion)]
[assembly: AssemblyInformationalVersion(ThisAssembly.InformationalVersion)]
[assembly: SatelliteContractVersion(ThisAssembly.Version)]

internal static class ThisAssembly
{
    internal const string Version = "4.0.0.0";
    internal const string InformationalVersion = "4.0.0.0";
}

internal static class AssemblyRef
{
    internal const string MicrosoftSilverlightPublicKeyToken = "b03f5f7f11d50a3a";
}
