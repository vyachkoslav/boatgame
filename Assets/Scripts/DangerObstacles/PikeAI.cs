using FishNet;
using UnityEngine;

public class PikeEnemy : MonoBehaviour
{
    [Header("Patrol Settings - INNER RING")] 
    [SerializeField] private float rotationSpeed = 180f;
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

    private Vector3 linearVelocity;
    private float lastAttackTime;
    private Vector3 retreatStartPosition;
    private Transform boat;
    
    private enum PikeState { Patrolling, Chasing, Retreating }
    private PikeState currentState = PikeState.Patrolling;

    private void Start()
    {
        if (patrolCenter == null) return;
        
        boat = GameObject.FindGameObjectWithTag("Boat").transform;
    }
    
    private void Update()
    {
        if (InstanceFinder.IsClientOnlyStarted) return;
        if (patrolCenter == null) return;
        
        float boatDistanceToCenter = Vector3.Distance(patrolCenter.position, boat.position);
        bool boatInOuterRing = boatDistanceToCenter <= aggroRadius;
        bool boatInVision = IsBoatInVisionCone(boat);
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

                Vector3 directionToBoat = (boat.position - transform.position);
                directionToBoat.y = 0;
                linearVelocity = directionToBoat.normalized * attackSpeed;
                
                if (directionToBoat != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToBoat);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.deltaTime);
                }
                
                break;
                
            case PikeState.Retreating:
                Vector3 retreatDirection = (transform.position - boat.position);
                retreatDirection.y = 0;
                linearVelocity = retreatDirection.normalized * retreatSpeed;
                
                if (retreatDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(retreatDirection);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.deltaTime);
                }
                
                float retreatDistanceTraveled = Vector3.Distance(retreatStartPosition, transform.position);
                // float distanceToPatrol = Vector3.Distance(transform.position, patrolCenter.position);
                
                if (retreatDistanceTraveled >= retreatDistance) // || distanceToPatrol <= patrolRadius)
                {
                    ReturnToPatrol();
                }
                break;
        }
        transform.position += linearVelocity * Time.deltaTime;
    }
    
    private void PatrolCircle()
    {
        int dir = clockwise ? -1 : 1;
        // float currentAngle = startAngle + dir * (Time.time * patrolSpeed * 20f) % 360;
        var pos = transform.position;
        Vector3 dirFromCenter = pos - patrolCenter.position; 
        float currentAngle = Mathf.Atan2(dirFromCenter.z, dirFromCenter.x) * Mathf.Rad2Deg;
        float targetAngle = currentAngle + dir;
        float rad = targetAngle * Mathf.Deg2Rad;
        
        Vector3 targetPos = patrolCenter.position + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * patrolRadius;
        targetPos.y = transform.position.y;
        
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        linearVelocity = direction.normalized * patrolSpeed;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime);
        }
    }
    
    private bool IsBoatInVisionCone(Transform targetBoat)
    {
        if (targetBoat == null) return false;
        
        Vector3 directionToBoat = (targetBoat.position - transform.position);
        directionToBoat.y = 0;
        float distanceToBoat = Vector3.Distance(transform.position, targetBoat.position);
        
        if (distanceToBoat > visionRange) return false;
        
        float angleToBoat = Vector3.Angle(transform.forward, directionToBoat);
        return angleToBoat <= visionAngle * 0.5f;
    }
    
    private void OnCollisionStay(Collision collision)
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

            currentState = PikeState.Retreating;
            retreatStartPosition = transform.position;
        }
    }
    
    private void ReturnToPatrol()
    {
        currentState = PikeState.Patrolling;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(patrolCenter.position, aggroRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(patrolCenter.position, patrolRadius);
        
        Vector3 forward = transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle * 0.5f, 0) * forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle * 0.5f, 0) * forward;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
    }
}