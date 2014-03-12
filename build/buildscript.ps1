Framework "4.5.1x86"
FormatTaskName (("-" * 20) + "[ {0} ]" + ("-" * 20))

$build_root = Split-Path $psake.build_script_file
$solution_dir = resolve-path "$build_root\.."
$out_dir = "$solution_dir\out"

$package_push_urls = @{ "myget"="https://www.myget.org/F/enyimmemcached2/api/v2/package"; "nuget"=""; }
$symbol_push_urls = @{ "myget"="https://nuget.symbolsource.org/MyGet/enyimmemcached2"; "nuget"="https://nuget.gw.symbolsource.org/Public/NuGet"; }

$solution_file = "$solution_dir\Enyim.Caching.sln"
$package_projects = ("Memcached\Memcached.csproj", "NLog\NLog.csproj") | % { join-Path $solution_dir $_ -Resolve }

#
# build script
#
Properties {

	#used by build/package
	$configuration = "Release"
	$platform = "Any CPU"

	#used by push
	$push_target = "myget"
	$push_symbols = $false
	$push_key = $null
}

Task Default -depends Clean, Package
Task Rebuild -depends Clean, Build -description "clean & build"

#
# CLEAN
#
#

Task Clean -description "removes all files created by the build process" {

	invoke-msbuild -target "Clean" -project $solution_file
}

#
# BUILD
#
#

Task Build -description "builds the projects" {

	invoke-msbuild -target "Build" -project $solution_file
}

#
# PACKAGE
#
#

Task Package -description "builds the nuget packages" -Depends Build {

	$package_projects | % {

		write-Host -ForegroundColor Green "`n  Creating package for $( [System.IO.Path]::GetFileNameWithoutExtension($_) )"

		invoke-msbuild -target "CreatePackage" -project $_
	}
}

#
# PUBLISH
#
#

Task Publish -description "publishes the nuget packages" -depends Package {

	$push_to = $package_push_urls[$push_target]
	assert ($push_to -ne $null) ("Invalid package host: $push_target")
	assert (![System.String]::IsNullOrEmpty($push_key)) ("Invalid or missing API key.")

	$extras = @("-apikey", "$push_key")
	if ($push_to -ne "") { $extras += "-source", $push_to }

	$package_projects | % {

		$p = join-Path (split-path -Parent $_) "bin\$configuration"

		gci $p *.nupkg | % { . "$solution_dir\.nuget\nuget.exe" push ($_.FullName) $extras }
	}
}

#
# UTILS
#
#

function invoke-msbuild($target, $props, $project) {

	Assert (![System.String]::IsNullOrEmpty($configuration)) ("Configuration must be specified")
	Assert (![System.String]::IsNullOrEmpty($platform)) ("Platform must be specified")

	$p = (
			($props + @{
				Configuration = $configuration;
				Platform = $platform;
				ILMergeEnabled = "True";
			}).GetEnumerator() | % { $_.Name + "=" + $_.Value }
		) -join ";"

	Exec { msbuild $project /t:$target /p:"$p" /v:m /nologo }
}

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
