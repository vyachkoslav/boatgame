using System;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    private static int maxHp = 2;
    private static int hp;

    // Events that can be subscribed to by HUD, particle system etc.
    public static event Action<int> OnBoatDamaged;
    public static event Action OnDeath;

    private void Awake()
    {
        hp = maxHp;
    }

    public static void TakeDamage(int damage)
    {
        hp -= damage;
        OnBoatDamaged?.Invoke(hp);

        if (hp <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
