using System.Collections.Generic;
using FishNet.Object;
using Network;
using UnityEngine;

public class HoleDam : NetworkPuzzle
{
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private List<GameObject> holesSpots = new();
    
    private readonly HashSet<GameObject> holes = new();

    protected override void OnPuzzleStart()
    {
        if (IsServerStarted)
            SpawnHolesServer();
    }

    protected override void OnPuzzleEnd(State state)
    {
        Debug.Log("Puzzle ended " + state);
    }
    
    private void SpawnHolesServer()
    {
        foreach (var spot in holesSpots)
        {
            var holeSpot = spot.transform;
            var nob = NetworkManager.GetPooledInstantiated(holePrefab, holeSpot.position, holeSpot.rotation, false);
            nob.GetComponent<Hole>().dam = this; // Sets itself as the dam that the hole belongs to
            Spawn(nob);
            holes.Add(nob.gameObject);
        }
    }

    [Server]
    public void RemoveHole(GameObject holeObject, GameObject blockerObject)
    {
        Despawn(blockerObject);
        Despawn(holeObject);

        holes.Remove(holeObject);

        if (holes.Count == 0)
            EndPuzzle(State.Success);
    }

    public override void OnStopServer()
    {
        foreach (var hole in holes)
        {
            Despawn(hole);
        }
    }
}
