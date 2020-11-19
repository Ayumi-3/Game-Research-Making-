using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationController : MonoBehaviour//, IReceiverObserver
{
    private UDPReceiver _UdpReceiver;
    private UDPTransmitter _UdpTransmitter;

    private void Awake()
    {
        _UdpReceiver = GetComponent<UDPReceiver>();
        //_UdpReceiver.SetObserver(this);
        _UdpTransmitter = GetComponent<UDPTransmitter>();
    }

    /// <summary>
    /// Send data immediately after receiving it.
    /// </summary>
    /// <param name="val"></param>
    //void IReceiverObserver.OnDataReceived(double[] val)
    //{
    //    _UdpTransmitter.Send(val);
    //}

    public void TransmitData(float val)
    {
        _UdpTransmitter.Send((double)val);
    }

    public float GetUdpData()
    {
        return _UdpReceiver.UDPdata;
    }

}