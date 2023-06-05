using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ANT_Managed_Library;
using System;



public class TrainerCapabilities {
    public int maximumResistance;
    public bool basicResistanceNodeSupport;
    public bool targetPowerModeSupport;
    public bool simulationModeSupport;
}

public class CommandStatus {

    public int lastReceivedCommandId;
    public int status;
    public int lastReceivedSequenceNumber;
    public byte byte_4;
    public byte byte_5;
    public byte byte_6;
    public byte byte_7;
}

public class UserConfiguration {

    public float bicycleWeight;
    public float userWeight;
}

public class AntDevice{

    public string name;
    public byte deviceType;
    public byte transType;
    public int period;
    public int deviceNumber;
    public int radiofreq;
}

public static class AntplusDeviceType{

    public const byte BikePower = 11;
    public const byte Control = 16;
    public const byte FitnessEquipment = 17;
    public const byte BikeSpeedCadence = 121;
    public const byte BikeCadence = 122;
    public const byte BikeSpeed = 123;
    public const byte Invalid = (byte)0xFF;
}

public class BikeTrainer : MonoBehaviour {

    public bool autoStartScan = true; //start scan on play
    public bool connected = false; //will be set to true once connected

    public bool autoConnectToFirstSensorFound = true;
    public List<AntDevice> scanResult;

    //the received sensor values - ANT+ Fitness Equipment chapter 8.6.7
    public float speed; //Instantaneous speed - 0xFFFF indicates invalid
    public float elapsedTime; //Accumulated value of the elapsed time since start of workout in seconds
    public int distanceTraveled; //Accumulated value of the distance traveled since start of workout in meters
    public int instantaneousPower; //Instantaneous power - 0xFFF indicates BOTH the instantaneous and accumulated power fields are invalid
    public int cadence; //Crank cadence – 0xFF indicates invalid

    private AntChannel backgroundScanChannel;
    private AntChannel deviceChannel;
    private byte[] pageToSend;
    public int deviceID = 0;
    void Start() {

        if (autoStartScan)
            StartScan();
    }

    //Starts a background scan to find the device
    public void StartScan() {

        Debug.Log("Looking for ANT + Fitness Equipment sensor");

        AntManager.Instance.Init();
        scanResult = new List<AntDevice>();
        backgroundScanChannel = AntManager.Instance.OpenBackgroundScanChannel(0);
        backgroundScanChannel.onReceiveData += ReceivedBackgroundScanData;
    }

    //If the device is found
    void ReceivedBackgroundScanData(Byte[] data) {

        byte deviceType = (data[12]); 

        switch (deviceType) {

            case AntplusDeviceType.FitnessEquipment: {
                    int deviceNumber = (data[10]) | data[11] << 8;
                    byte transType = data[13];
                    foreach (AntDevice d in scanResult) {
                        if (d.deviceNumber == deviceNumber && d.transType == transType)
                            return;
                    }

                    Debug.Log("FitnessEquipmentfound " + deviceNumber);

                    AntDevice foundDevice = new AntDevice();
                    foundDevice.deviceType = deviceType;
                    foundDevice.deviceNumber = deviceNumber;
                    foundDevice.transType = transType;
                    foundDevice.period = 8192;
                    foundDevice.radiofreq = 57;
                    foundDevice.name = "FitnessEquipment(" + foundDevice.deviceNumber + ")";
                    scanResult.Add(foundDevice);
                    if (autoConnectToFirstSensorFound) {
                        ConnectToDevice(foundDevice);
                    }
                    break;
                }

            default: {

                    break;
                }
        }

    }
    //Creates a channel with a found device
    void ConnectToDevice(AntDevice device) {
        AntManager.Instance.CloseBackgroundScanChannel();
        byte channelID = AntManager.Instance.GetFreeChannelID();
        deviceChannel = AntManager.Instance.OpenChannel(ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00, channelID, (ushort)device.deviceNumber, device.deviceType, device.transType, (byte)device.radiofreq, (ushort)device.period, false);
        connected = true;
        deviceChannel.onReceiveData += Data;
        deviceChannel.onChannelResponse += ChannelResponse;
        deviceChannel.hideRXFAIL = true;
    }


    //Received Data
    int prevDistance = 0;
    float prevTime = 0;
    bool firstDistanceInfo;
    bool firstTimeInfo;
    public void Data(Byte[] data) {

        // General FE Data - ANT+ Fitness Equipment chapter 8.5.2 
        if (data[0] == 16) {

            if (prevDistance < data[3])
                distanceTraveled += data[3] - prevDistance;

            if (firstDistanceInfo == false) {
                distanceTraveled -= data[3];
                firstDistanceInfo = true;
            }
            prevDistance = data[3];

            // elapsed time

            if (prevTime < data[2])
                elapsedTime += (data[2] - prevTime) * 0.25f;

            if (firstTimeInfo == false) {
                elapsedTime -= data[2] * 0.25f;
                firstTimeInfo = true;
            }

            prevTime = data[2];

            //speed in kmph
            speed = ((data[4]) | data[5] << 8) * 0.0036f;
        }

        //Specific Trainer/Stationary Bike Data - ANT+ Fitness Equipment chapter 8.6.7
        if (data[0] == 25) {  

            cadence = data[2];
            int nibble2 = (byte)(data[6] & 0xf);
            instantaneousPower = (data[5]) | nibble2 << 8;
        }
    }

    //set the trainer in resistance mode - ANT+ Fitness Equipment chapter 8.8.1
    public void SetTrainerResistance(int resistance) {
        if (!connected)
        return;

        pageToSend = new byte[8] { 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(resistance * 2) };//unit is 0.50%
        deviceChannel.sendBroadcastData(pageToSend);
    }

    //set the trainer in target power - ANT+ Fitness Equipment chapter 8.8.2
    public void SetTrainerTargetPower(int targetpower) {
        if (!connected)
            return;

        Debug.Log("sending target power of " + targetpower + " watts to trainer");
        byte LSB = (byte)(targetpower * 4);
        byte MSB = (byte)((targetpower * 4 >> 8));

        pageToSend = new byte[8] { 0x31, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, LSB, MSB };//unit is 0.25w
        deviceChannel.sendBroadcastData(pageToSend);
    }

    //set the trainer in simulation mode - ANT+ Fitness Equipment chapter 8.8.4
    public void SetTrainerSlope(int slope) {
        if (!connected)
            return;
        slope = Mathf.Clamp(slope, -5, 20);

        slope += 200;
        
        slope *= 100;
        int grade = (int)slope;
        byte gradeLsb = (byte)(grade);
        byte gradeMsb = (byte)(grade >> 8);
        byte[] pageToSend = new byte[8] { 0x33, 0xFF, 0xFF, 0xFF, 0xFF, gradeLsb, gradeMsb, 0xFF };//unit is 0.01%
        deviceChannel.sendBroadcastData(pageToSend);
    }

    void ChannelResponse(ANT_Response response) {

        if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.EVENT_TRANSFER_TX_FAILED_0x06) {
            if (pageToSend != null)
                deviceChannel.sendAcknowledgedData(pageToSend); //send the page again if the transfer failed

        }
    }


}
