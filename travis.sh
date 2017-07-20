#!/usr/bin/env bash

# exit on first error
set -e

PROJECTS=(Tomorrow.Core.Abstractions.UnitTests Tomorrow.Core.UnitTests Tomorrow.Core.Json.UnitTests Tomorrow.InProcess.UnitTests)

dotnet restore

for project in $PROJECTS
do
  cd $project
  dotnet build
  dotnet test
  cd -
done
