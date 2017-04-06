@echo off
:: Copyright (c) .NET Foundation and contributors. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

setlocal
cd /d %~dp0
powershell -NoProfile -NoLogo -ExecutionPolicy Bypass -Command "& \"%~dp0build.ps1\" %*; exit $LastExitCode;"
