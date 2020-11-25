using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationController : MonoBehaviour, IReceiverObserver
{
    UDPReceiver _UdpReceiver;
    UDPTransmitter _Udpransmitter;

    public float ReceivedData;

    private void Awake()
    {
        _UdpReceiver = GetComponent<UDPReceiver>();
        _UdpReceiver.SetObserver(this);
        _Udpransmitter = GetComponent<UDPTransmitter>();
    }

    /// <summary>
    /// Send data immediately after receiving it.
    /// </summary>
    /// <param name="val"></param>
    void IReceiverObserver.OnDataReceived(double[] val)
    {
        //_Udpransmitter.Send(val);
        ReceivedData = (float)val[0];
    }

    public void SendTriggerToMatlab(bool isStart)
    {
        if (isStart)
        {
            _Udpransmitter.Send(1);
        }
        else
        {
            _Udpransmitter.Send(-1);
        }
    }

}