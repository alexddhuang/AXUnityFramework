using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace AXUnityFramework {

namespace Network {

public interface IDataParser
{
    void Parse(byte[] bytes);
}

public class TCPClient : MonoBehaviour
{
    public string IP = "127.0.0.1";
    public int PORT = 0;

    private TcpClient _client;

    private Thread _receivingDataThread;

    private IDataParser _parser;

    public void Send(byte[] bytes) 
    {
        if (_client != null && _client.Connected) {
            var stream = _client.GetStream();
            if (stream.CanWrite) {
                short size = (short) (bytes.Length + 2);
                byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(size));
                byte[] packet = new byte[header.Length + bytes.Length];
                header.CopyTo(packet, 0);
                bytes.CopyTo(packet, header.Length);

                stream.Write(packet, 0, packet.Length);
            }
        }
    }

    public void Listen(IDataParser parser) 
    {
        _parser = parser;
    }

    // Start is called before the first frame update
    void Start()
    {
        try {
            _client = new TcpClient(IP, PORT);
        } catch (SocketException e) {
            Debug.Log($"Socket exception: {e}");
        }

        if (_client != null && _client.Connected) {
            try {
                _receivingDataThread = new Thread(new ThreadStart(ReceivingData));
                _receivingDataThread.IsBackground = true;
                _receivingDataThread.Start();
            } catch (Exception e) {
                Debug.Log($"Exception: {e}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ReceivingData() 
    {
        Debug.Log("Receiving data");
        var stream = _client.GetStream();
        while (_client.Connected) {
            if (stream.CanRead && _client.Available > 0) {
                Debug.Log($"{_client.Available} bytes are available to be read.");

                var header = new byte[2];
                var num = stream.Read(header, 0, header.Length);
                if (num > 0) {
                    short size = BitConverter.ToInt16(header, 0);
                    size = IPAddress.NetworkToHostOrder(size);

                    var buffer = new byte[size - header.Length];
                    var offset = 0;
                    var remain = buffer.Length;
                    do {
                        offset += stream.Read(buffer, offset, remain);
                        remain = buffer.Length - offset;
                    } while (offset < buffer.Length);

                    if (_parser != null) {
                        _parser.Parse(buffer);
                    }
                }
            }
        }

        _receivingDataThread.Abort();
    }
}

} // End of namespace Network

} // End of namespace AXUnityFramework