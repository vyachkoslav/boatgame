using System;
using System.Collections;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    private static int maxHp = 2;
    private static int hp;

    private Rigidbody boatRb;
    [SerializeField] private float sinkTime = 4.0f;
    [SerializeField] private float sinkSpeed = 0.05f;
    
    // Events that can be subscribed to by HUD, particle system etc. to trigger effects
    public static event Action<int> OnBoatDamaged;
    public static event Action OnDeath;
    public static event Action OnFinishedSinking;

    private void OnEnable()
    {
        OnDeath += SinkBoat;
    }

    private void OnDisable()
    {
        OnDeath -= SinkBoat;
    }

    private void Awake()
    {
        hp = maxHp;
        boatRb = GetComponent<Rigidbody>();
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

    private void SinkBoat()
    {
        StartCoroutine(SinkRoutine());
    }

    private IEnumerator SinkRoutine()
    {
        boatRb.isKinematic = true; // Disables physics
        var timeGoal = Time.time + sinkTime;
        while (Time.time < timeGoal)
        {
            transform.position -= new Vector3(0, sinkSpeed * Time.deltaTime, 0);
            yield return null;
        }
        OnFinishedSinking?.Invoke();
    }
}
