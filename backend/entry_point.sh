#!/bin/sh

export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
export DOTNET_USE_POLLING_FILE_WATCHER=true

export PATH=$PATH:/root/.dotnet/tools

chmod -R g+w /opt/data

dotnet ef migrations add InitialCreate
dotnet ef migrations add UpdateWithStages
dotnet ef database update

dotnet run