using System;
using System.Collections.Generic;
using Sining.Event;
using Sining.Network;

namespace Sining.Module
{
    [ComponentSystem]
    public class NetOuterComponentServiceAwakeSystem : AwakeSystem<NetOuterComponent, string>
    {
        protected override void Awake(NetOuterComponent self, string address)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType, address);
        }
    }

    [ComponentSystem]
    public class
        NetOuterComponentNetworkProtocolAwakeSystem : AwakeSystem<NetOuterComponent, NetworkProtocolType>
    {
        protected override void Awake(NetOuterComponent self, NetworkProtocolType networkProtocol)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.SetNetworkProtocol(Enum.GetName(typeof(NetworkProtocolType), networkProtocol));
            self.Awake(self.NetworkProtocolType);
        }
    }

    [ComponentSystem]
    public class NetOuterComponentNetworkProtocolStringAwakeSystem : AwakeSystem<NetOuterComponent, string, string>
    {
        protected override void Awake(NetOuterComponent self, string address, string networkProtocol)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.SetNetworkProtocol(networkProtocol);
            self.Awake(self.NetworkProtocolType, address);
        }
    }

    [ComponentSystem]
    public class
        NetOuterComponentNetworkUrlsProtocolAwakeSystem : AwakeSystem<NetOuterComponent, IEnumerable<string>, string>
    {
        protected override void Awake(NetOuterComponent self, IEnumerable<string> urls, string networkProtocol)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.SetNetworkProtocol(networkProtocol);
            self.Awake(self.NetworkProtocolType, urls);
        }
    }

    [ComponentSystem]
    public class NetOuterComponentConnectAwakeSystem : AwakeSystem<NetOuterComponent>
    {
        protected override void Awake(NetOuterComponent self)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType);
        }
    }

    public class NetOuterComponent : NetworkComponent
    {
        public NetworkProtocolType NetworkProtocolType { get; private set; } = NetworkProtocolType.HTTP;

        public void SetNetworkProtocol(string networkProtocol)
        {
            switch (networkProtocol)
            {
                case "TCP":
                    NetworkProtocolType = NetworkProtocolType.TCP;
                    break;
                case "WebSocket":
                    NetworkProtocolType = NetworkProtocolType.WebSocket;
                    break;
                case "HTTP":
                    NetworkProtocolType = NetworkProtocolType.HTTP;
                    break;
                default:
                    throw new Exception($"No ServerType found for {networkProtocol}");
            }
        }
    }
}