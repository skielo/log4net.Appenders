ApiKey=$1

/home/travis/build/skielo/log4net.Appenders/.nuget/NuGet.exe pack ./log4net.Appenders.nuspec -Verbosity detailed

/home/travis/build/skielo/log4net.Appenders/.nuget/NuGet.exe push ./log4net.Appenders.*.nupkg -Verbosity detailed -ApiKey $ApiKey