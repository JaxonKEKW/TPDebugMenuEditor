dotnet publish -c Release -o publish -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=false --self-contained true -r win-x64 -p:IncludeNativeLibrariesForSelfExtract=true