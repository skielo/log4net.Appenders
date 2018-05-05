![Build Status](https://travis-ci.org/skielo/log4net.Appenders.svg?branch=master) [![NuGet version](https://badge.fury.io/nu/log4net.Appenders.svg)](https://www.nuget.org/packages/log4net.Appenders/)  [![Stories](https://badge.waffle.io/skielo/log4net.Appenders.svg?columns=To%20Do,In%20Progress&style=flat)](https://waffle.io/skielo/log4net.Azure) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/skielo/log4net.Appenders/blob/master/LICENSE.md) #log4net.Appenders

Transfer all your logs to the [Azure Table or Blob Storage](http://azure.microsoft.com/de-de/services/storage/) via Appender for [log4Net](https://logging.apache.org/log4net/)

## Install
Add To project via NuGet:  
1. Right click on a project and click 'Manage NuGet Packages'.  
2. Search for 'log4net.Appenders' and click 'Install'.  

## Disclaimer

This is an adaptation of this original component (https://github.com/stemarie/log4net.Azure)

## Configuration
### Table Storage 
Every log entry is stored in a separate row.

	<appender name="AzureTableAppender" type="log4net.Appender.AzureTableAppender, log4net.Appender.Azure">
	   <param name="TableName" value="testLoggingTable"/>
	   <!-- You can either specify a connection string or use the ConnectionStringName property instead -->
	   <param name="ConnectionString" value="UseDevelopmentStorage=true"/>
	   <!--<param name="ConnectionStringName" value="GlobalConfigurationString" />-->
	   <!-- You can specify this to make each LogProperty as separate Column in TableStorage, 
		Default: all Custom Properties were logged into one single field -->
	   <param name="PropAsColumn" value="true" />
	   <!-- You can specify this to make each LogProperty as separate Column in TableStorage, 
		Default: all Custom Properties were logged into one single field -->
	   <param name="PropAsColumn" value="true" />
	   <param name="PartitionKeyType" value="LoggerName" />
	 </appender>
	
* <b>TableName:</b>  
  Name of the table in Table Storage
* <b>ConnectionString:</b>  
  the full Azure Storage connection string
* <b>ConnectionStringName:</b>  
  Name of a connection string specified under connectionString
* <b>PropAsColumn(optional):</b>  
  Default: all properties were written in a single field(default).  
  If you specifiy this with the value true then each custom log4net property is logged as separate column/field in the table.  
  Remember that Table storage has a Limit of 255 Properties ([see here](https://azure.microsoft.com/en-us/documentation/articles/storage-table-design-guide/#about-the-azure-table-service)).
* <b>PartitionKeyType(optional):</b>  
  Default "LoggerName": (each logger gets his own partition in Table Storage)  
  "DateReverse": order by Date Reverse to see the latest items first ([How to order elements by date reverse](http://gauravmantri.com/2012/02/17/effective-way-of-fetching-diagnostics-data-from-windows-azure-diagnostics-table-hint-use-partitionkey/))


### Async Table Storage 
Every log entry is stored in a separate row in an Azure Table Storage with an additional mechanism to push log events after certain amount of time.

    <appender name="AsyncTableAzureAppender" type="log4net.Appender.AsyncAzureTableAppender, log4net.Appender.Azure">
      <param name="TableName" value="testDynamicLoggingTable"/>
      <param name="ConnectionString" value="UseDevelopmentStorage=true"/>
      <param name="PropAsColumn" value="true"/>
      <param name="PartitionKeyType" value="DateReverse"/>
	  <!--Define the interval to retry-->
      <param name="RetryWait" value="00:00:04" --> 4 seconds TimeSpan />
	  <!--Define the interval to flush the events "00:02:00" --> 2 minutes TimeSpan-->
      <param name="FlushInterval" value="00:02:00" />
    </appender>
	
* <b>TableName:</b>  
  Name of the table in Table Storage
* <b>ConnectionString:</b>  
  the full Azure Storage connection string
* <b>ConnectionStringName:</b>  
  Name of a connection string specified under connectionString
* <b>PropAsColumn(optional):</b>  
  Default: all properties were written in a single field(default).  
  If you specifiy this with the value true then each custom log4net property is logged as separate column/field in the table.  
  Remember that Table storage has a Limit of 255 Properties ([see here](https://azure.microsoft.com/en-us/documentation/articles/storage-table-design-guide/#about-the-azure-table-service)).
* <b>PartitionKeyType(optional):</b>  
  Default "LoggerName": (each logger gets his own partition in Table Storage)  
  "DateReverse": order by Date Reverse to see the latest items first ([How to order elements by date reverse](http://gauravmantri.com/2012/02/17/effective-way-of-fetching-diagnostics-data-from-windows-azure-diagnostics-table-hint-use-partitionkey/))
* <b>BatchSize(optional):</b>
  Default size: 100
  Defines the size of the batch to be processed.
* <b>RetryCount(optional):</b>
  Default count: 5
  Defines the number of retry
* <b>RetryWait(optional):</b>
  Default wait time: 5 seconds
  Defines the wait time to retry.
* <b>FlushInterval(optional):</b>
  Default flush interval: 1 minute
  Defines the interval to flush the events.

	
### BlobStorage
Every log Entry is stored as separate XML file.

    <appender name="AzureBlobAppender" type="log4net.Appender.AzureBlobAppender, log4net.Appender.Azure">
      <param name="ContainerName" value="testloggingblob"/>
      <param name="DirectoryName" value="logs"/>
      <!-- You can either specify a connection string or use the ConnectionStringName property instead -->
      <param name="ConnectionString" value="UseDevelopmentStorage=true"/>
      <!--<param name="ConnectionStringName" value="GlobalConfigurationString" />-->
    </appender>
	
* <b>ContainerName:</b>  
  Name of the container in Blob Storage	
* <b>DirectoryName:</b>  
  Name of the folder in the specified container
* <b>ConnectionString:</b>  
  the full Azure Storage connection string
* <b>ConnectionStringName:</b>  
  Name of a connection string specified under connectionString

### AppendBlobStorage
Every log Entry is stored as separate XML file.

    <appender name="AzureAppendBlobAppender" type="log4net.Appender.AzureAppendBlobAppender, log4net.Appender.Azure">
      <param name="ContainerName" value="testloggingblob"/>
      <param name="DirectoryName" value="logs"/>
      <!-- You can either specify a connection string or use the ConnectionStringName property instead -->
      <param name="ConnectionString" value="UseDevelopmentStorage=true"/>
      <!--<param name="ConnectionStringName" value="GlobalConfigurationString" />-->
    </appender>
	
* <b>ContainerName:</b>  
  Name of the container in Blob Storage	
* <b>DirectoryName:</b>  
  Name of the folder in the specified container
* <b>ConnectionString:</b>  
  the full Azure Storage connection string
* <b>ConnectionStringName:</b>  
  Name of a connection string specified under connectionString

### APIAppender
Every log Entry is pushed to an endpoint API.

    <appender name="APIAppender1" type="log4net.Appender.API.APIAppender, log4net.Appender.API">
	  <!-- endpoint url -->
      <param name="RequestUrl" value="/posts"/>
	  <!-- base url -->
      <param name="BaseUrl" value="https://jsonplaceholder.typicode.com"/>
      <param name="BufferSize" value="1" />
    </appender>

* <b>RequestUrl:</b>  
  Request segment of the API endpoint
* <b>BaseUrl:</b>  
  Base url of the endpoint
* <b>BasicUser:</b>  
  User to include the basic authentincation header
* <b>BasicPass:</b>  
  Pass to include the basic authentincation header

### FailoverAppender
This appender has configured two appenders, the primary where is going to push the logs and a failover appender
in case something goes wrong.

    <appender name="FailoverAppender" type="log4net.Appender.API.FailoverAppender, log4net.Appender.API">
        <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%date [%thread] %-5level %logger - %message%newline"/>
        </layout>
 
        <!--This is a custom test appender that will always throw an exception -->
        <!--The first and the default appender that will be used.-->
        <PrimaryAppender type="log4net.Azure.console.ExceptionThrowerAppender" >
            <ThrowExceptionForCount value="1" />
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date [%thread] %-5level %logger - %message%newline"/>
            </layout>        
        </PrimaryAppender>
 
        <!--This appender will be used only if the PrimaryAppender has failed-->
        <FailOverAppender type="log4net.Appender.RollingFileAppender">
            <file value="log.txt"/>
            <rollingStyle value="Size"/>
            <maxSizeRollBackups value="10"/>
            <maximumFileSize value="100mb"/>
            <appendToFile value="true"/>
            <staticLogFileName value="true"/>
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date [%thread] %-5level %logger - %message%newline"/>
            </layout>
        </FailOverAppender>
    </appender>

* <b>PrimaryAppender:</b>  
  Primary appender to push logs
* <b>FailOverAppender:</b>  
  Secondary appender in case something goes wrong.

## Run Tests

For now to run the Unit Test you must have installed and running the **Azure Storage Emulator** (https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)

## Author

**Ezequiel Reyno** (http://github.com/skielo)

## Credits

**Karell Ste-Marie** (https://github.com/stemarie) Creator of the component, unfortunately the original project is not longer supported.

