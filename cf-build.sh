#!/bin/sh
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 6.0 -InstallDir ./dotnet6
./dotnet6/dotnet --version
./dotnet6/dotnet publish LukeParkerDev.Web/LukeParkerDev.Web.csproj -c Release -o output