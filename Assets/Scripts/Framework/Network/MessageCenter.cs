using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using pb = global::Google.Protobuf;

namespace AXUnityFramework {

namespace Network {

public interface IMessageHandler
{
    void Handle(short cmd, byte[] buffer);
}

public class MessageCenter : MonoBehaviour, IDataParser
{
    public TCPClient client;

    private Dictionary<short, IMessageHandler> _handlers = new Dictionary<short, IMessageHandler>();

    public void Register(short cmd, IMessageHandler handler)
    {
        _handlers.Add(cmd, handler);
    }

    public void Parse(byte[] bytes) 
    {
        byte[] head = new byte[2];
        Buffer.BlockCopy(bytes, 0, head, 0, 2);
        
        short cmd = BitConverter.ToInt16(head, 0);
        cmd = IPAddress.NetworkToHostOrder(cmd);

        byte[] body = new byte[bytes.Length - 2];
        Buffer.BlockCopy(bytes, 2, body, 0, bytes.Length - 2);

        IMessageHandler handler;
        var ok = _handlers.TryGetValue(cmd, out handler);
        if (ok) {
            handler.Handle(cmd, body);
        }
    }

    public void Send(short cmd, pb.IMessage message)
    {
        byte[] head = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(cmd));
        int bodySize = message.CalculateSize();
        byte[] body = new byte[bodySize];
        
        pb.CodedOutputStream output = new pb.CodedOutputStream(body);
        message.WriteTo(output);

        byte[] buffer = new byte[head.Length + body.Length];
        head.CopyTo(buffer, 0);
        body.CopyTo(buffer, head.Length);

        client.Send(buffer);
    }

    // Start is called before the first frame update
    void Start()
    {
        client.Listen(this);
    }
}

} // End of namespace Network

} // End of namespace AXUnityFramework