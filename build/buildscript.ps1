Framework "4.6x64"
FormatTaskName (("-" * 20) + "[ {0} ]" + ("-" * 20))

$build_root = Split-Path $psake.build_script_file
$solution_dir = resolve-path "$build_root\.."
$out_dir = "$solution_dir\!out\"
$nuget_exe = resolve-path "$build_root\nuget.exe"

$package_push_urls = @{ "myget"="https://www.myget.org/F/enyimmemcached2/api/v2/package"; "nuget"=""; }
$symbol_push_urls = @{ "myget"="https://nuget.symbolsource.org/MyGet/enyimmemcached2"; "nuget"="https://nuget.gw.symbolsource.org/Public/NuGet"; }

$solution_file = "$solution_dir\Enyim.Caching.sln"
$package_projects = (
						"Memcached",
						"NLog",
						"log4net",
						"Configuration"
					) | % { resolve-path "$solution_dir\$_\$_.csproj" }

#
# build script
#
Properties {

	#used by build/package
	$configuration = "Release"
	$platform = "Any CPU"
	$verbosity = "q"

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

Task Build -description "builds the projects" -depends _Restore {

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
# LOCAL COPY
#
#

Task LocalCopy -description "builds the nuget packages and copies them to the output directory" -Depends Package {
	if (!(Test-Path $out_dir)) { mkdir $out_dir > $null }

	find-packages | copy -destination $out_dir
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

	find-packages | % { . $nuget_exe push ($_.FullName) $extras }
}

#
# NUGET RESTORE
#
#

Task _Restore -description "restores nuget packages" {
	Exec {
		pushd $solution_dir
		. $nuget_exe restore
		popd
	}
}

#
# UTILS
#
#

function guess-sha() {
	$sha = git log --pretty=format:%h -1 2> $null

	if ($LASTEXITCODE -ne 0) { $sha = $env:APPVEYOR_REPO_COMMIT }

	return $sha
}

function guess-branch() {
	$branch = git rev-parse --abbrev-ref HEAD 2> $null

	if ($LASTEXITCODE -ne 0) { $branch = $env:APPVEYOR_REPO_BRANCH }

	return $branch
}

function non-empty($v) { $v | ? { $_ } }

function get-informal-version($v) {

	$sha = guess-sha
	$branch = guess-branch
	$counter = $env:APPVEYOR_BUILD_NUMBER

	# version+branch.sha.counter
	return ((non-empty @($v, ((non-empty @($branch, $sha, $counter)) -join "."))) -join "+")
}

function prepare-version-numbers() {
	$version = gc "$solution_dir\VERSION"
	assert ($version -match "\d+.\d+(.\d+)?") ("Invalid version number: $version")

	$normal = $Matches[0]
	$informal = get-informal-version $version

	return @{ ProjectVersion=$normal; ProjectInformalVersion=$informal }
}

function invoke-msbuild($target, $props, $project) {

	Assert (![System.String]::IsNullOrEmpty($configuration)) ("Configuration must be specified")
	Assert (![System.String]::IsNullOrEmpty($platform)) ("Platform must be specified")

	$p = (
			($props + (prepare-version-numbers) + @{
				Configuration = $configuration;
				Platform = $platform;
				ILMergeEnabled = "True";
				SolutionDir = "$solution_dir\\";
			}).GetEnumerator() | % { $_.Name + "=""$( $_.Value )""" }
		) -join ";"

	$v = $verbosity
	if ([String]::IsNullOrWhiteSpace($v)) { $v = "q" }

	$msbuildargs = @(
		"""$project""",
		"/t:$target",
		"/p:$p",
		"/v:$v",
		"/nologo"
	)

	if (![String]::IsNullOrWhiteSpace($env:AppVeyorCI)) {
		$msbuildargs += @( "/logger:C:\Program^ Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" )
	}

	msbuild $msbuildargs
}

function find-packages
{
	$package_projects | % {

		$p = join-Path (split-path -Parent $_) "bin\$configuration"

		gci $p *.nupkg
	}
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
