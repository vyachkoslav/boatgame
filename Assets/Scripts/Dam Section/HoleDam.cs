using System.Collections.Generic;
using FishNet.Object;
using Network;
using UnityEngine;

public class HoleDam : NetworkPuzzle
{
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private List<GameObject> holesSpots = new();
    
    private readonly HashSet<GameObject> holes = new();

    [SerializeField] private GameObject successfulBlockVFXPrefab;
    [SerializeField] private float vfxDestroyDelay = 2f;

    protected override void OnPuzzleStart()
    {
        if (IsServerStarted)
            SpawnHolesServer();
            // Updates the hole count on the HUD
            if (HoleCounterHUD.Instance != null)
                HoleCounterHUD.Instance.UpdateHoleCount(holes.Count);
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
        if (!holes.Remove(holeObject)) return;

        PlayBlockVFX(blockerObject.transform.position);
        Despawn(blockerObject);
        Despawn(holeObject);
        if (HoleCounterHUD.Instance != null)
            HoleCounterHUD.Instance.UpdateHoleCount(holes.Count);

        if (holes.Count == 0)
            EndPuzzle(State.Success);
    }

    [ObserversRpc]
    public void PlayBlockVFX(Vector3 position)
    {
        // Creates the particle effect at the point of collision or where the trigger happened, based on parameter
        GameObject particleEffect = Instantiate(successfulBlockVFXPrefab, position, successfulBlockVFXPrefab.transform.rotation);

        Destroy(particleEffect, vfxDestroyDelay);
    }

    public override void OnStopServer()
    {
        foreach (var hole in holes)
        {
            Despawn(hole);
        }
        if (HoleCounterHUD.Instance != null)
            HoleCounterHUD.Instance.ClearHoleCount();
    }
}
