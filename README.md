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
![image](https://user-images.githubusercontent.com/17733188/189773129-942cbcc9-e6b1-41f4-b5ff-36a2fb6e265c.png)

Custom histogram measures the duration of the consume events
![image](https://user-images.githubusercontent.com/17733188/193685742-4ef1f80e-7845-4f10-9c99-8c56d549679e.png)
