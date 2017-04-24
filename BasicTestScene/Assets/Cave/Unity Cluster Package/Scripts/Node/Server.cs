﻿using System;
using UnityEngine;
using AwesomeSockets.Domain.Sockets;
using AwesomeSockets.Sockets;

namespace UnityClusterPackage
{ 
    class Server : NetworkNode
    {
        public int targetClientNumber = 1;

        private ISocket listenSocket;

        public Server(int targetClientNumber)
        {
            this.targetClientNumber = targetClientNumber;
        }

        public override void Connect()
        {
            listenSocket = AweSock.TcpListen(NodeInformation.serverPort + 1);
            while (connections.Count < targetClientNumber)
            {
                connections.Add(AweSock.TcpAccept(listenSocket));
                InitializeClient(connections[connections.Count - 1]);
            }
        }


        void InitializeClient(ISocket connection)
        {
           ParticleSynchronizer.InitializeFromServer(this, connection);
        }


        public override void Disconnect()
        {
            base.Disconnect();
            listenSocket.Close();
        }


        public override void FinishFrame()
        {
            int counter = 0;
            while (counter < connections.Count)
            {
                SynchroMessage message = WaitForNextMessage(connections[counter]);

                if (message.type == SynchroMessageType.FinishedRendering)
                    counter++;
                else
                    throw new Exception("Received unexpected message.");
            }
        }

    }
}
