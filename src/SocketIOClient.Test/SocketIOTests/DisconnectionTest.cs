﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocketIOClient.Test.SocketIOTests
{
    public abstract class DisconnectionTest
    {
        protected abstract ISocketIOCreateable SocketIOCreator { get; }

        public virtual async Task ServerDisconnect()
        {
            string reason = null;
            var client = new SocketIO(SocketIOCreator.Url, new SocketIOOptions
            {
                Reconnection = false,
                Query = new Dictionary<string, string>
                {
                    { "token", SocketIOCreator.Token }
                },
                EIO = SocketIOCreator.EIO
            });

            Assert.IsFalse(client.Connected);
            Assert.IsTrue(client.Disconnected);

            client.OnConnected += async (sender, e) =>
            {
                Assert.IsTrue(client.Connected);
                Assert.IsFalse(client.Disconnected);
                await client.EmitAsync("sever disconnect");
            };
            client.OnDisconnected += (snder, e) => reason = e;

            await client.ConnectAsync();

            await Task.Delay(200);
            await client.DisconnectAsync();

            Assert.IsFalse(client.Connected);
            Assert.IsTrue(client.Disconnected);
            Assert.AreEqual("io server disconnect", reason);
        }

        public virtual async Task ClientDisconnect()
        {
            string reason = null;
            var client = new SocketIO(SocketIOCreator.Url, new SocketIOOptions
            {
                Reconnection = false,
                Query = new Dictionary<string, string>
                {
                    { "token", SocketIOCreator.Token }
                },
                EIO = SocketIOCreator.EIO
            });

            Assert.IsFalse(client.Connected);
            Assert.IsTrue(client.Disconnected);

            client.OnDisconnected += (snder, e) => reason = e;

            await client.ConnectAsync();
            await Task.Delay(200);
            await client.DisconnectAsync();

            Assert.IsFalse(client.Connected);
            Assert.IsTrue(client.Disconnected);
            Assert.AreEqual("io client disconnect", reason);
        }
    }
}
