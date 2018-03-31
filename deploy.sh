ApiKey=$1

./.nuget/NuGet.exe pack ./log4net.Appenders.nuspec -Verbosity detailed

./.nuget/NuGet.exe push ./log4net.Appenders.*.nupkg -Verbosity detailed -ApiKey $ApiKey