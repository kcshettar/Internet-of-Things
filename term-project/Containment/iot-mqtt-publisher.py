 #!/usr/bin/python3

#required libraries
import sys                                 
import ssl
import json
import paho.mqtt.client as mqtt

#implementation
import requests

# for motion sensor
import RPi.GPIO as GPIO
import time
from datetime import datetime
import picamera
camera = picamera.PiCamera()


#called while client tries to establish connection with the server 
def on_connect(mqttc, obj, flags, rc):
    if rc==0:
        print ("Subscriber Connection status code: "+str(rc)+" | Connection status: successful")
        mqttc.subscribe("$aws/things/led_test/shadow/update/accepted", qos=0)
    elif rc==1:
        print ("Subscriber Connection status code: "+str(rc)+" | Connection status: Connection refused")

#called when a topic is successfully subscribed to
def on_subscribe(mqttc, obj, mid, granted_qos):
    print("Subscribed: "+str(mid)+" "+str(granted_qos)+"data"+str(obj))

#called when a message is received by a topic
def on_message(mqttc, obj, msg):
    print("Received message from topic: "+msg.topic+" | QoS: "+str(msg.qos)+" | Data Received: "+str(msg.payload))

#creating a client with client-id=mqtt-test
mqttc = mqtt.Client(client_id="cgao")

mqttc.on_connect = on_connect
mqttc.on_subscribe = on_subscribe
mqttc.on_message = on_message

#Configure network encryption and authentication options. Enables SSL/TLS support.
#adding client-side certificates and enabling tlsv1.2 support as required by aws-iot service
mqttc.tls_set(ca_certs="/home/pi/Downloads/rootCA.pem.crt",
	            certfile="/home/pi/Downloads/708441901a-certificate.pem.crt",
	            keyfile="/home/pi/Downloads/708441901a-private.pem.key",
              tls_version=ssl.PROTOCOL_TLSv1_2, 
              ciphers=None)

#connecting to aws-account-specific-iot-endpoint
mqttc.connect("a1qup6k0p06lhy.iot.us-west-2.amazonaws.com", port=8883) #AWS IoT service hostname and portno

#automatically handles reconnecting
#start a new thread handling communication with AWS IoT
mqttc.loop_start()

sensor = 18



rc=0
print("___________________________________________________________________________")
try:
    if rc == 0:
        
        camera.capture('/home/pi/Desktop/image1.jpg')
        url = 'http://10.0.0.196:8080/api/fileupload/uploadfile'
        files = {'UploadedImage': open('/home/pi/Desktop/image1.png', 'rb')}
        output = requests.post(url, files=files)
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(sensor,GPIO.OUT)
        print(output.text)
        
        data={}
        if output.text == "\"open\"":
            data['position'] = output.text
            GPIO.output(sensor, GPIO.HIGH)
            print("LED on")
            data['motion']=output.text
        else:
            data['position'] = output.text
            GPIO.output(sensor, GPIO.LOW)
            print("LED off")
            data['motion']=output.text
           
        #data['motion']=output.text
        data['time']=datetime.now().strftime('%Y/%m/%d %H:%M:%s')
        print(data)
        playload = '{"state":{"reported":'+json.dumps(data)+'}}'
        print(playload)

        #the topic to publish to
        #the names of these topics start with $aws/things/thingName/shadow.
        msg_info = mqttc.publish("$aws/things/led_test/shadow/update", playload, qos=1)
        
        time.sleep(1.5)  

except KeyboardInterrupt:
    pass

GPIO.cleanup()
