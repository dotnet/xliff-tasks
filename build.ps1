# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information

[CmdletBinding()]
param(
    [Switch]
    [Bool] 
    $Release = $false,

    [Parameter(Position=0, ValueFromRemainingArguments=$true)]
    $ExtraParameters
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$RepoRoot = "$PSScriptRoot"
$DotnetInstallDir = "$RepoRoot\.dotnet"

$Configuration = if ($Release) { "Release" } else { "Debug" }
$LogDir = "$RepoRoot\bin\$Configuration\Logs"

$CommitCount = & git rev-list --count HEAD

$env:XDG_DATA_HOME="$RepoRoot\.nuget\packages"
$env:NUGET_PACKAGES="$RepoRoot\.nuget\packages"
$env:NUGET_HTTP_CACHE_PATH="$RepoRoot\.nuget\packages"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

New-Item -Type Directory -Force $DotnetInstallDir  | Out-Null
New-Item -Type Directory -Force $LogDir | Out-Null

Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" -OutFile "$DotnetInstallDir\dotnet-install.ps1"
& $DotnetInstallDir/dotnet-install.ps1 -Version "1.0.1" -InstallDir "$DotnetInstallDir"

$env:PATH="$DotnetInstallDir;$env:PATH"

& dotnet msbuild build\build.proj /v:normal /flp:Verbosity=Diag`;LogFile=$LogDir\msbuild.log /p:CommitCount=$CommitCount /p:Configuration=$Configuration $ExtraParameters 