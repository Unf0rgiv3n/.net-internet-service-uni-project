FROM python:3.10-alpine
WORKDIR /usr/src/app
RUN python -m pip install pika --upgrade
COPY . .
CMD ["python", "./generator.py"]