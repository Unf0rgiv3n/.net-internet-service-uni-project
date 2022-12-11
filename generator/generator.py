import pika
import time
import json
import random
from enum import Enum
from abc import ABC, abstractmethod


class SensorType(Enum):
    HUMIDITY = 0
    INSOLATION = 1
    TEMPERATURE = 2
    NOISE = 3

class Sensor(ABC):
    SensorId: int
    Name: str
    Desc: str
    Measurement: int
    Type: type

    @abstractmethod
    def generate_measurement(self):
        raise NotImplementedError

class HumiditySensor(Sensor):
    def __init__(self, Id, Name, Desc) -> None:
        super().__init__()
        self.SensorId = Id
        self.Name = Name
        self.Desc = Desc
        self.Measurement = 0
        self.Type = SensorType.HUMIDITY.name
        self.Unit = '%'

    def generate_measurement(self):
        self.Measurement = random.randrange(101)

class InsolationSensor(Sensor):
    def __init__(self, Id, Name, Desc) -> None:
        super().__init__()
        self.SensorId = Id
        self.Name = Name
        self.Desc = Desc
        self.Measurement = 0
        self.Type = SensorType.INSOLATION.name
        self.Unit = 'W/m^2'
    
    def generate_measurement(self):
        self.Measurement = random.randrange(1501)

class TemperatureSensor(Sensor):
    def __init__(self, Id, Name, Desc) -> None:
        super().__init__()
        self.SensorId = Id
        self.Name = Name
        self.Desc = Desc
        self.Measurement = 0
        self.Type = SensorType.TEMPERATURE.name
        self.Unit = 'C'

    def generate_measurement(self):
        self.Measurement = random.randrange(71)

class NoiseSensor(Sensor):
    def __init__(self, Id, Name, Desc) -> None:
        super().__init__()
        self.SensorId = Id
        self.Name = Name
        self.Desc = Desc
        self.Measurement = 0
        self.Type = SensorType.NOISE.name
        self.Unit = 'dB'

    def generate_measurement(self):
        self.Measurement = random.randrange(251)

def send_msg(sensor: Sensor, channel):
    queue_name = f'sensor'
    channel.basic_publish(exchange='',
        routing_key=queue_name,
        body=json.dumps(sensor.__dict__))

def generate_sensor():
    Id = 1
    rooms = ['living room', 'kitchen', 'bedroom', 'garage', 'bathroom', 'basement', 'garden', 'attic']
    while True:
        type_num = random.randrange(4)
        Name = f'{SensorType(type_num).name}-{Id}'
        Desc = f'Monitored room: {random.choice(rooms)}'

        if SensorType(type_num) == SensorType.HUMIDITY:
            yield HumiditySensor(Id, Name, Desc)
        elif SensorType(type_num) == SensorType.INSOLATION:
            yield InsolationSensor(Id, Name, Desc)
        elif SensorType(type_num) == SensorType.TEMPERATURE:
            yield TemperatureSensor(Id, Name, Desc)
        elif SensorType(type_num) == SensorType.NOISE:
            yield NoiseSensor(Id, Name, Desc)

        Id = Id + 1

def generate_sensor_list():
    sensor_gen = generate_sensor()
    sensor_list = []
    for i in range(30):
        sensor = next(sensor_gen)
        sensor_list.append(sensor)
    return sensor_list

if __name__ == '__main__':
    sensor_list = generate_sensor_list()
    time.sleep(30)

    connection = pika.BlockingConnection(pika.ConnectionParameters('rabbit'))
    channel = connection.channel()
    channel.queue_declare(queue='sensor')

    for i in range(3000):
        rnd_sensor = random.choice(sensor_list)
        rnd_sensor.generate_measurement()
        send_msg(rnd_sensor, channel)
        time.sleep(10)
    
    connection.close()
    