ApiKey=$1

./.nuget/nuget pack ./log4net.Appenders.nuspec -Verbosity detailed

./.nuget/nuget push ./log4net.Appenders.*.nupkg -Verbosity detailed -ApiKey $ApiKey