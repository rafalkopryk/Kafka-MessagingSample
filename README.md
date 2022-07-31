# Kafka-MessagingSample

An example of using apache kafka to send messages between applications

stack:
- messaging.publisher - sample asp.net application to publish message
- messaging.consumer - sample asp.net application to consume message

- docker
- elastic APM - tracing
- elasticsearch - logs (in this case)
- kibana - visualization data
- Apache Kafka - distributed streaming platform

event.messaging.publisher.messagePublished.v1 event on Elastic APM.
![image](https://user-images.githubusercontent.com/17733188/182048574-22c37dce-aa35-48b1-98ec-feb71c649710.png)

