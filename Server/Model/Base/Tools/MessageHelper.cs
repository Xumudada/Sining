using System;
using System.Collections.Generic;
using Sining.Config;
using Sining.Module;
using Sining.Network;

namespace Sining.Tools
{
    public static class MessageHelper
    {
        private static readonly Dictionary<int, string> AddressCache = new Dictionary<int, string>();

        public static void Send(this IMessage message, int serverId)
        {
            NetInnerComponent.Instance.GetSession(GetAddress(serverId)).Send(message);
        }

        public static STask<TResponse> Call<TResponse>(this IRequest request, int serverId) where TResponse : IResponse
        {
            return NetInnerComponent.Instance.GetSession(GetAddress(serverId)).Call<TResponse>(request);
        }

        private static string GetAddress(int serverId)
        {
            if (AddressCache.TryGetValue(serverId, out var address)) return address;

            var serverConfig = ServerConfigData.Instance.GetConfig(serverId);
            if (serverConfig == null)
            {
                throw new Exception("没有找到该服务器的配置文件");
            }

            address = $"{serverConfig.InnerIP}:{serverConfig.InnerPort}";
            AddressCache.Add(serverId, address);
            return address;
        }
    }
}