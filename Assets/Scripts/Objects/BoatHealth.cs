using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

public class BoatHealth : NetworkBehaviour
{
    public static BoatHealth Instance { get; private set; }

    private static int maxHp = 2;
    public static int MaxHp => maxHp;
    [AllowMutableSyncType]
    private SyncVar<int> hp = new SyncVar<int>();

    private Rigidbody boatRb;
    [SerializeField] private Transform waterChecker;
    private float timeUpsideDown;
    [SerializeField] private float upsideDownLimit = 4.0f;
    [SerializeField] private float sinkTime = 4.0f;
    [SerializeField] private float sinkSpeed = 0.075f;
    
    // Events that can be subscribed to by HUD, particle system etc. to trigger effects
    public static event Action<int> OnBoatDamaged;
    public static event Action OnDeath;
    public static event Action OnFinishedSinking;

    private void Awake()
    {
        // Sets self as instance if it doesn't exist yet
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        hp.Value = maxHp;
        boatRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var waterCheckerPos = waterChecker.position;
        var waterLevel = GlobalObjects.Water.GetWaterPointHeight(waterCheckerPos);

        // Check if raft is upside down
        if (waterCheckerPos.y < waterLevel)
        {
            timeUpsideDown += Time.deltaTime;

            // Raft takes max damage when upside down long enough
            if (timeUpsideDown >= upsideDownLimit && hp.Value > 0)
            {
                TakeDamage(maxHp);
            }
        }
        else
        {
            // When water checker is not underwater, the timer ticks back down, with minimum value of 0
            timeUpsideDown = Mathf.Clamp(timeUpsideDown - Time.deltaTime, 0, upsideDownLimit);
        }
    }

    public override void OnStartClient()
    {
        PersistentHUD.Instance.UpdateBoatHp(hp.Value);
    }

    private void OnEnable()
    {
        OnDeath += SinkBoat;
        hp.OnChange += OnChangeHealth;
    }

    private void OnDisable()
    {
        OnDeath -= SinkBoat;
        hp.OnChange -= OnChangeHealth;
    }

    public void TakeDamage(int damage)
    {
        hp.Value -= damage;
        OnBoatDamaged?.Invoke(hp.Value);

        if (hp.Value <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private void OnChangeHealth(int previous, int next, bool asServer)
    {
        PersistentHUD.Instance.UpdateBoatHp(hp.Value);
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
