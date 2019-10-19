param (
    [string]$server =  'localhost:9092',
    [string]$topicname = $null
)

.\bin\windows\kafka-topics.bat --create --bootstrap-server $server --replication-factor 1 --partitions 1 --topic $topicname