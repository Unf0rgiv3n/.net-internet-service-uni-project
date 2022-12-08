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
    id: int
    name: str
    desc: str
    measurement: int
    type: type

    @abstractmethod
    def generate_measurement(self):
        raise NotImplementedError

        # if self.type == type.HUMIDITY:
        #     self.unit = '%'
        # elif self.type == type.INSOLATION:
        #     self.unit = 'w/m^2'
        # elif self.type == type.TEMPERATURE:
        #     self.unit = 'C'
        # elif self.type == type.NOISE:
        #     self.unit = 'dB'

class HumiditySensor(Sensor):
    def __init__(self, id, name, desc) -> None:
        super().__init__()
        self.id = id
        self.name = name
        self.desc = desc
        self.measurement = 0
        self.type = SensorType.HUMIDITY.name
        self.unit = '%'

    def generate_measurement(self):
        self.measurement = random.randrange(101)

class InsolationSensor(Sensor):
    def __init__(self, id, name, desc) -> None:
        super().__init__()
        self.id = id
        self.name = name
        self.desc = desc
        self.measurement = 0
        self.type = SensorType.INSOLATION.name
        self.unit = 'W/m^2'
    
    def generate_measurement(self):
        self.measurement = random.randrange(1501)

class TemperatureSensor(Sensor):
    def __init__(self, id, name, desc) -> None:
        super().__init__()
        self.id = id
        self.name = name
        self.desc = desc
        self.measurement = 0
        self.type = SensorType.TEMPERATURE.name
        self.unit = 'C'

    def generate_measurement(self):
        self.measurement = random.randrange(71)

class NoiseSensor(Sensor):
    def __init__(self, id, name, desc) -> None:
        super().__init__()
        self.id = id
        self.name = name
        self.desc = desc
        self.measurement = 0
        self.type = SensorType.NOISE.name
        self.unit = 'dB'

    def generate_measurement(self):
        self.measurement = random.randrange(251)

def send_msg(sensor: Sensor, channel):
    queue_name = f'sensor'
    channel.basic_publish(exchange='',
        routing_key=queue_name,
        body=json.dumps(sensor.__dict__))

def generate_sensor():
    id = 1
    rooms = ['living room', 'kitchen', 'bedroom', 'garage', 'bathroom', 'basement', 'garden', 'attic']
    while True:
        type_num = random.randrange(4)
        name = f'{SensorType(type_num).name}-{id}'
        desc = f'Monitored room: {random.choice(rooms)}'

        if SensorType(type_num) == SensorType.HUMIDITY:
            yield HumiditySensor(id, name, desc)
        elif SensorType(type_num) == SensorType.INSOLATION:
            yield InsolationSensor(id, name, desc)
        elif SensorType(type_num) == SensorType.TEMPERATURE:
            yield TemperatureSensor(id, name, desc)
        elif SensorType(type_num) == SensorType.NOISE:
            yield NoiseSensor(id, name, desc)

        id = id + 1

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

    connection = pika.BlockingConnection(pika.ConnectionParameters('si_180152_rabbit'))
    channel = connection.channel()
    channel.queue_declare(queue='sensor')

    for i in range(2000):
        rnd_sensor = random.choice(sensor_list)
        rnd_sensor.generate_measurement()
        send_msg(rnd_sensor, channel)
        time.sleep(10)
    
    connection.close()
    