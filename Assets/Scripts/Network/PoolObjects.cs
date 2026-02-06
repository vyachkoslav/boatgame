using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace Network
{
    public class PoolObjects : NetworkBehaviour
    {
        [Serializable]
        public struct ObjectWithCount
        {
            public NetworkObject Object;
            public int Count;
        }
        
        [SerializeField] private List<ObjectWithCount> objects;

        public override void OnStartNetwork()
        {
            foreach (var obj in objects)
            {
                NetworkManager.CacheObjects(obj.Object, obj.Count, false);
            }
        }
    }
}
