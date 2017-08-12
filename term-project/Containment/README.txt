University of Massachusetts, Lowell
Computer Science
Internet-Of-Things

Description:
Final-Project;

***********************************************************************************************************************************************************
The main purpose of the project is to identify the whether the door is open or closed using Machine Learning where the algorithm trains the model.
One of the primary applications of this project is that it can examine maufactured products in factories for detecting detects and other shortcomings.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> iot-mqtt-publisher.py
This file contains python code for connecting to the Amazon Web Services(AWS).
The python code also includes the connection to the server (our computer for this project).
The camera code for capturing the image of the door and led code for glowing the led when the door is open and not glowing the led when the door is closed is included in this code. 
We have previously created a thing in the AWS IoT and connected to it using this code.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> mqttsubscriber.py
This file contains python code for making the Thing in the AWS IoT the Broker.
We have previously created a thing in AWS IoT and made it the broker using this code.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> Program.cs
This C# file contains Evaluation Single Image funtion which acccepts the input file image and processes it with cifar10.ResNet.cmf model file and returns output as whether the image is open or closed based upon the input.
This file contains Machine Learning Microsoft CNTK CNN (Convolution Neural Networks).
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> FileUploadController.cs
This file is a ASP.Net C# file which conatians the service API for uploading the image captured from Raspberry-Pi camera and is responsible for invoking Evaluation Single Image funtion present in Program.cs
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> CntkBitmapExtension.cs.
This file contains extensions for resize function and Parallel extract CHW functions required for Image processing.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> ImageHandsOn.cntk
This file is resonsible for Traning and Testing the model i.e creating model file from 1000's of images of open and closed doors.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> cifar10.ResNet.cmf
This is trained model file required for Machine Learning algorithm.
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-> ValPerc.cs
This file contains getters and setters required for program.cs filr.