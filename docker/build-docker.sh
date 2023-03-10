#!/usr/bin/env bash
dotnet publish ../src/MatchingEngine.Service/MatchingEngine.Service.csproj --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer