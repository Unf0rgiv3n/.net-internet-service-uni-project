import pika
import time

connection = pika.BlockingConnection(pika.ConnectionParameters('si_180152_rabbit'))
channel = connection.channel()
channel.queue_declare(queue='test')
for i in range(50):
    channel.basic_publish(exchange='',
    routing_key='test',
    body='testmsg')
    time.sleep(10)
connection.close()