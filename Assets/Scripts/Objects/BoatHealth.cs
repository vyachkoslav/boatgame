using System;
using System.Collections;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    private static int maxHp = 2;
    private static int hp;

    private Rigidbody boatRb;
    [SerializeField] private float sinkTime = 4.0f;
    [SerializeField] private float sinkPerFrame = 0.0005f;
    private float elapsedTime = 0.0f;
    private bool isSinking = false;
    
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

    private void FixedUpdate()
    {
        if (isSinking)
        {
            elapsedTime += Time.fixedDeltaTime;
            transform.position -= new Vector3(0, sinkPerFrame, 0);
        }
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
        isSinking = true;
        boatRb.isKinematic = true; // Disables physics
        StartCoroutine(WaitUntilSunk());
    }

    private IEnumerator WaitUntilSunk()
    {
        yield return new WaitUntil(() => elapsedTime >= sinkTime);
        OnFinishedSinking?.Invoke();
    }
}
