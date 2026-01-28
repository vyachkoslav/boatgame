using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Object;
using FishNet.Object;
using UnityEngine;

namespace Network
{
    public class PoolObjects : MonoBehaviour
    {
        [Serializable]
        public struct ObjectWithCount
        {
            public NetworkObject Object;
            public int Count;
        }
        
        [SerializeField] private List<ObjectWithCount> objects;

        private void Start()
        {
            foreach (var obj in objects)
            {
                InstanceFinder.NetworkManager.CacheObjects(obj.Object, obj.Count, false);
            }
        }
    }
}
