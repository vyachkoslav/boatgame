using UnityEngine;

public class PikeEnemy : MonoBehaviour
{
    [Header("Patrol Settings - INNER RING")]
    [SerializeField] private float patrolRadius = 1.5f;
    [SerializeField] private float patrolSpeed = 0.8f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private Transform patrolCenter;
    
    [Header("Aggro Settings - OUTER RING")]
    [SerializeField] private float aggroRadius = 2f;
    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private float visionRange = 2.5f;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float retreatSpeed = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float retreatDistance = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float knockbackForce = 50f;
    
    [Header("Water")]
    [SerializeField] private float waterLevelY = 0.049f;
    
    private Rigidbody rb;
    private float startAngle;
    private float lastAttackTime;
    private bool isRetreating = false;
    private Vector3 retreatStartPosition;
    
    private enum PikeState { Patrolling, Chasing, Retreating }
    private PikeState currentState = PikeState.Patrolling;

     private void Awake()
    {
    rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (patrolCenter == null) return;
        
        Vector3 offset = transform.position - patrolCenter.position;
        offset.y = 0;
        startAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }
    
    private void Update()
    {
        if (rb == null) return;
        if (patrolCenter == null) return;
        
        // Find boat if not set
        GameObject boat = GameObject.FindGameObjectWithTag("Boat");
        if (boat == null) return;
        Transform boatTransform = boat.transform;
        
        KeepAtWaterLevel();
        
        float distanceToBoat = Vector3.Distance(transform.position, boatTransform.position);
        bool boatInOuterRing = distanceToBoat <= aggroRadius;
        bool boatInVision = IsBoatInVisionCone(boatTransform);
        bool shouldChase = boatInOuterRing && boatInVision;
        
        switch (currentState)
        {
            case PikeState.Patrolling:
                PatrolCircle();
                if (shouldChase) currentState = PikeState.Chasing;
                break;
                
            case PikeState.Chasing:
                if (!shouldChase)
                {
                    ReturnToPatrol();
                    break;
                }
                
                Vector3 directionToBoat = (boatTransform.position - transform.position).normalized;
                rb.linearVelocity = directionToBoat * attackSpeed;
                
                if (directionToBoat != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToBoat);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
                }
                
                if (isRetreating)
                {
                    currentState = PikeState.Retreating;
                    retreatStartPosition = transform.position;
                }
                break;
                
            case PikeState.Retreating:
                Vector3 retreatDirection = (transform.position - boatTransform.position).normalized;
                rb.linearVelocity = retreatDirection * retreatSpeed;
                
                if (retreatDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(retreatDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
                }
                
                float retreatDistanceTraveled = Vector3.Distance(retreatStartPosition, transform.position);
                float distanceToPatrol = Vector3.Distance(transform.position, patrolCenter.position);
                
                if (retreatDistanceTraveled >= retreatDistance || distanceToPatrol <= patrolRadius)
                {
                    ReturnToPatrol();
                }
                break;
        }
    }
    
    private void PatrolCircle()
    {
        float dir = clockwise ? -1f : 1f;
        float currentAngle = startAngle + dir * (Time.time * patrolSpeed * 20f) % 360;
        float rad = currentAngle * Mathf.Deg2Rad;
        
        Vector3 targetPos = patrolCenter.position + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * patrolRadius;
        targetPos.y = waterLevelY;
        
        Vector3 direction = (targetPos - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
        }
    }
    
    private bool IsBoatInVisionCone(Transform targetBoat)
    {
        if (targetBoat == null) return false;
        
        Vector3 directionToBoat = (targetBoat.position - transform.position).normalized;
        float distanceToBoat = Vector3.Distance(transform.position, targetBoat.position);
        
        if (distanceToBoat > visionRange) return false;
        
        float angleToBoat = Vector3.Angle(transform.forward, directionToBoat);
        return angleToBoat <= visionAngle * 0.5f;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (currentState != PikeState.Chasing) return;
        if (Time.time < lastAttackTime + attackCooldown) return;
        
        if (collision.gameObject.CompareTag("Boat"))
        {
            lastAttackTime = Time.time;
            BoatHealth.Instance.TakeDamage(attackDamage);
            
            Rigidbody boatRb = collision.rigidbody;
            if (boatRb != null)
            {
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = 0;
                boatRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
            
            isRetreating = true;
        }
    }
    
    private void ReturnToPatrol()
    {
        currentState = PikeState.Patrolling;
        isRetreating = false;
        
        Vector3 offset = transform.position - patrolCenter.position;
        offset.y = 0;
        startAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }
    
    private void KeepAtWaterLevel()
    {
        Vector3 pos = transform.position;
        pos.y = waterLevelY;
        transform.position = pos;
        
        if (rb != null)
        {
            Vector3 rbPos = rb.position;
            rbPos.y = waterLevelY;
            rb.position = rbPos;
        }
    }
}