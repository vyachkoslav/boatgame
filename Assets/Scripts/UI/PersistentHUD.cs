using FishNet.Object;
using TMPro;
using UnityEngine;

public class PersistentHUD : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI boatHpIndicator;

    private void Awake()
    {
        boatHpIndicator.text = "Raft HP: " + BoatHealth.MaxHp;
    }

    private void OnEnable()
    {
        BoatHealth.OnBoatDamaged += UpdateBoatHp;
    }

    private void OnDisable()
    {
        BoatHealth.OnBoatDamaged -= UpdateBoatHp;
    }

    [Server]
    public void UpdateBoatHp(int hp)
    {
        CmdUpdateBoatHp(hp);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CmdUpdateBoatHp(int hp)
    {
        boatHpIndicator.text = "Raft HP: " + hp;
    }
}
