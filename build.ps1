<#
.SYNOPSIS
This is the entry point of the build process.

.DESCRIPTION
By default the build script rebuilds and signs the assemblies,
then creates the Nuget packages. As for the the signing
  a) you must provide your own key, otherwise
  b) the assemblies will be delay-signed with the official key.

For case a) you must have a 'private.snk' key in the solution root;
copy an existing one, or generate with 'sn -k'.

If you use choose the delay-signed option, you must disable the
strong name verification using 'sn -Vr' (see the MSDN for more information).

.PARAMETER Tasks
The comma-separated list of task(s) to be run from the build script.
The following task name can be passed to the build script:
  - Clean
  - Build
  - Package
  - Publish

If omitted the 'Default' task will be run.

.PARAMETER Configuration
Use a specific solution configuration, instead of 'Release'.

.PARAMETER Platform
Use a specific target platform when building, instead of 'Any CPU'.

.PARAMETER Target
Specify where to publish the packages. The following values are accepted: 'nuget', 'myget' (default).

.PARAMETER Symbols
Specify whether the debug symbols should be published alongside the packages.

.PARAMETER ApiKey
The API Key used to publish the packages.

.EXAMPLE
C:\PS> .\build.ps1 Build

Run the build with default settings.

.EXAMPLE
C:\PS> .\build.ps1 Publish -ApiKey aaaa-bbbb-cccc-dddd -target "nuget"

Build and publish the packages to nuget.

.LINK
"Create a Public/Private Key Pair" (http://msdn.microsoft.com/en-us/library/6f05ezxy)
"Delay Signing an Assembly" (http://msdn.microsoft.com/en-us/library/t07a3dye)
#>

param(
	[string]$Tasks = "Default",
	[string]$Configuration = "Release",
	[string]$Platform = "Any CPU",
	[string]$Target = "myget",
	[Switch]$Symbols = $false,
	[string]$ApiKey = $null,
	[string]$Verbosity = $null
)

import-module .\build\psake\psake.psm1
try {

	if ($ApiKey.Length -eq 0) {
		$fn = "${Target}.apikey"

		if (test-path $fn) { $ApiKey = gc $fn }
	}

	invoke-psake -nologo -buildFile .\build\buildscript.ps1 -TaskList $Tasks -properties @{configuration=$configuration; platform=$platform; push_target=$target; push_symbols=$symbols; push_key = $APIKey; verbosity = $Verbosity }
}
finally { remove-module psake }

<#

	Copyright (c) Attila Kiskó, enyim.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.

#>
