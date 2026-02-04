using System.Collections.Generic;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using Utility;

public class Checkpoints : NetworkBehaviour
{
    public static Dictionary<string, int> LastCheckpoints = new();
    private static bool registered;

    [SerializeField] private Transform player;
    [SerializeField] private List<TriggerCallback> checkpoints = new();
    
    private string scene;

    public override void OnStartServer()
    {
        if (!registered)
        {
            ServerManager.OnServerConnectionState += OnServerConnectionState;
            registered = true;
        }
        
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].id = i;
            checkpoints[i].onTriggerEnter.AddListener(OnCheckpointEnter);
        }

        scene = gameObject.scene.name;
        
        if (!LastCheckpoints.TryGetValue(scene, out var checkpoint)) return;
        Debug.Log($"Loading checkpoint: {scene}-{checkpoint}");
        player.position = checkpoints[checkpoint].transform.position;
    }

    private static void OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log("Clearing checkpoints");
            LastCheckpoints.Clear();
        }
    }

    private void OnCheckpointEnter(int id, Collider other)
    {
        if (!other.CompareTag("Boat")) return;
        if (LastCheckpoints.TryGetValue(scene, out var checkpoint) && checkpoint >= id) return;
        
        Debug.Log($"Checkpoint: {scene}-{id}");
        LastCheckpoints[scene] = id;
    }
}
