using UnityEngine;

using AXUnityFramework.Network;
using pb = global::Google.Protobuf;

public class LoginHandler : MonoBehaviour, IMessageHandler 
{
    public MessageCenter messageCenter;
    public TCPClient client;

    private bool _testMessageSent = false;

    public void Handle(short cmd, byte[] buffer)
    {        
        Playermgr.LoginRsp message = new Playermgr.LoginRsp();

        pb.CodedInputStream input = new pb.CodedInputStream(buffer);
        message.MergeFrom(input);

        Debug.Log($"Handle {cmd}");
        Debug.Log($"result: {message.Result}");
        Debug.Log($"username: {message.Username}");
        Debug.Log($"userid: {message.Userid}");
        Debug.Log($"glod: {message.Glod}");
    }

    void Start()
    {
        messageCenter.Register(100, new LoginHandler());
    }

    void Update()
    {
        if (client.IsConnected() && !_testMessageSent) {
            SendTestMessage();
        }
    }

    private void SendTestMessage() 
    {
        Playermgr.LoginReq req = new Playermgr.LoginReq();
        req.Username = "test1";
        req.Password = "123456";

        messageCenter.Send(100, req);

        _testMessageSent = true;
    }
}