#!/usr/bin/env bash
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

# Exit if any subcommand or pipeline returns non-zero status
set -e

# Change directories to this script's home
cd "$(dirname "${BASH_SOURCE[0]}")"

# Determine canonical directory name of current directory/repository root
REPO_ROOT="$(pwd -P)"

# Some things depend on HOME and it may not be set
if [ -z "$HOME" ]; then
    export HOME="$REPO_ROOT/.home"
    mkdir -p $HOME
fi

CONFIGURATION=Debug

while true; do
    arg="$(echo $1 | tr '[:upper:]' '[:lower:]')"
    case $arg in
        -release)
            CONFIGURATION=Release
            shift
            ;;
        *)
            break
            ;;
    esac
done

LOG_DIR=$REPO_ROOT/bin/$CONFIGURATION/Logs
COMMIT_COUNT=$(git rev-list --count HEAD)

export XDG_DATA_HOME="$REPO_ROOT/.nuget/packages"
export NUGET_PACKAGES="$REPO_ROOT/.nuget/packages"
export NUGET_HTTP_CACHE_PATH="$REPO_ROOT/.nuget/packages"
export DOTNET_INSTALL_DIR="$REPO_ROOT/.dotnet"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

mkdir -p $DOTNET_INSTALL_DIR
mkdir -p $LOG_DIR

curl -sSL https://raw.githubusercontent.com/dotnet/cli/release/2.0.0/scripts/obtain/dotnet-install.sh | bash /dev/stdin --install-dir $DOTNET_INSTALL_DIR --version 2.0.0

PATH="$DOTNET_INSTALL_DIR:$PATH"

dotnet msbuild build/build.proj /v:normal /flp:Verbosity=Detailed\;LogFile=$LOG_DIR/msbuild.log /p:CommitCount=$COMMIT_COUNT /p:Configuration=$CONFIGURATION "$@"

