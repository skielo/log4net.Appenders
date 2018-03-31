ApiKey=$1

nuget pack ./log4net.Appenders.nuspec -Verbosity detailed

nuget push ./log4net.Appenders.*.nupkg -Verbosity detailed -ApiKey $ApiKey