﻿using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public class ServerWorld : WorldBase<ServerSimulationSystemGroup>
    {
        protected ServerOptions Options { get; }
        protected Entity NetworkStatusEntity;
        protected  NetworkStreamReceiveSystem NetworkStreamReceiveSystem;

        public ServerWorld(ServerOptions options = null, List<Type> systems = null)
            : base(options ?? new ServerOptions(), ClientServerBootstrap.CreateServerWorld,
                systems.AppendTypesWithAttribute<ServerSystemAttribute>())
        {
            Options = options ?? new ServerOptions();

            NetworkStatusEntity =
                World.EntityManager.CreateEntity(typeof(NetworkStatus));

            NetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>();

            if (Options.ConnectOnSpawn)
            {
                Listen();
            }
        }

        public void Listen()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);

            NetworkStreamReceiveSystem.Listen(endPoint);

            NetworkStatus networkStatus = new NetworkStatus
            {
                State = NetworkStreamReceiveSystem.Driver.Listening
                    ? NetworkState.Listening
                    : NetworkState.ListenFailed
            };

            World.EntityManager.SetComponentData(NetworkStatusEntity, networkStatus);
        }

        public void Disconnect()
        {
            World.DestroySystem(NetworkStreamReceiveSystem);
            NetworkStreamReceiveSystem = World.CreateSystem<NetworkStreamReceiveSystem>();
                World.GetExistingSystem<NetworkReceiveSystemGroup>()
                .AddSystemToUpdateList(NetworkStreamReceiveSystem);
                World.GetExistingSystem<NetworkReceiveSystemGroup>().SortSystemUpdateList();
        }
    }
}