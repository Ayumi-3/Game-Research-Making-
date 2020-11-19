/*
-----------------------
UDP Object
-----------------------
Code pulled from here and modified
 [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]

Unity3D to MATLAB UDP communication 

Modified by: Sandra Fang 
2016
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class Read2UDP : MonoBehaviour
{

    //Ports
    public int portLocal = 8000;
    public int portLocal1 = 8001;

    // Create necessary UdpClient objects
    UdpClient client;
    UdpClient client1;

    // Receiving Thread
    Thread receiveThread;
    private bool onRecieve = true;

    private float timeshift = 0;
    public float dataTempChanged;
    public float timeTempChanged;
    private static List<float> dataChanged = new List<float>();
    private static List<float> timeChanged = new List<float>();
    public static Dictionary<string, string> UDPdata = new Dictionary<string, string>();


    // start from Unity3d
    public void Start()
    {
        print("Read2UDP start");
        Init();
    }


    // Initialization code
    private void Init()
    {
        // Initialize (seen in comments window)
        // print ("UDP Object init()");

        // Create local client
        client = new UdpClient(portLocal);
        client1 = new UdpClient(portLocal1);

        // local endpoint define (where messages are received)
        // Create a new thread for reception of incoming messages
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        print("UDP is listening");
    }

    // Receive data, update packets received
    private void ReceiveData()
    {
        do
        {
            try
            {
                IPEndPoint IP8000 = new IPEndPoint(IPAddress.Any, portLocal);
                IPEndPoint IP8001 = new IPEndPoint(IPAddress.Any, portLocal1);
                byte[] data = client.Receive(ref IP8000);
                byte[] data1 = client1.Receive(ref IP8001);
                dataTempChanged = (float)BitConverter.ToDouble(data, 0);
                timeTempChanged = (float)BitConverter.ToDouble(data1, 0);

                UDPdata["data"] = dataTempChanged.ToString("f3");
                UDPdata["time"] = dataTempChanged.ToString("f3");

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        } while (onRecieve);
    }


    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        onRecieve = false;
        if (receiveThread != null)
            receiveThread.Abort();
        client.Close();
        client1.Close();
        print("close all");
    }
}