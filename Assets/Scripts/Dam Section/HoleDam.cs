using System.Collections.Generic;
using UnityEngine;

public class HoleDam : Puzzle
{
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private List<GameObject> holesSpots = new List<GameObject>();
    private List<GameObject> holes = new List<GameObject>();

    protected override void StartPuzzle()
    {
        SpawnHoles();
    }

    private void SpawnHoles()
    {
        GameObject currentHole;

        foreach (GameObject holeSpot in holesSpots)
        {
            currentHole = Instantiate(holePrefab, holeSpot.transform.position, holeSpot.transform.rotation);
            currentHole.GetComponent<Hole>().dam = this; // Sets itself as the dam that the hole belongs to

            holes.Add(currentHole);
        }
    }

    public void RemoveHole(GameObject holeObject)
    {
        holes.Remove(holeObject);

        if (holes.Count == 0)
        {
            EndPuzzle();
        }
    }
}
