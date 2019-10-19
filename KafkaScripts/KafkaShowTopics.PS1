param (
    [string]$server =  'localhost:9092'
)

.\bin\windows\kafka-topics.bat --list --bootstrap-server $server