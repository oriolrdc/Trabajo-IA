using UnityEngine;
using UnityEngine.AI;



public class Enemy : MonoBehaviour
{

    private Transform _player;
    [SerializeField] private Transform _StarterPoint;
    private NavMeshAgent _enemyAgent;

    [SerializeField] private float _detectionRange = 7f;

    private float _searchTimer;

    [SerializeField] private float _searchWaitTime = 15f;

    [SerializeField] private float _searchRadius = 10f; 

    Vector3 _playerLastPositionKnown;
    [SerializeField] private float _detectionAngle = 90f;

    private float _timer;
    private float _Wai;


    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();

        _player = GameObject.FindWithTag("Player").transform;
    }
   public enum EnemyState // Alamcena diferentes estados posibles
    {
        Patrolling,

        Chasing,

        Searching,

        Waiting,

        Attacking
    }

    public EnemyState currentState; // Almacena el estado actual que tiene

    [SerializeField] private Transform[] _patrolPoints;

    void Start()
    {
        currentState = EnemyState.Patrolling;
        PatrolInOrder();
    }

    void Update()
    {
        switch(currentState)
        {
            case EnemyState.Patrolling:

                Patrol();

            break;

            case EnemyState.Chasing:

                Chase();

            break;

            case EnemyState.Searching:

                Search();

            break;

            case EnemyState.Waiting:

                Waiting();

            break;

            case EnemyState.Attacking:

                
                Attacking();
                
            break;

            default:

                Patrol();

            break;
        }
    }

    void Patrol()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }
        if(_enemyAgent.remainingDistance < 0.5f)
        {
            PatrolInOrder();
        }
    }
    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Searching;
        }
        _enemyAgent.SetDestination(_player.position);

        _playerLastPositionKnown = _player.position;
    }

    void Search()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        _searchTimer += Time.deltaTime;

        if(_searchTimer < _searchWaitTime)
        {
            if(_enemyAgent.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if(RandomSearchPoint(_playerLastPositionKnown, _searchRadius, out randomPoint))
                {
                    _enemyAgent.SetDestination(randomPoint);
                }
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            _searchTimer = 0;
        }
    }

    void Waiting()
    {

    }

    void Attacking()
    {

    }

    void PatrolInOrder()
    {
        _enemyAgent.SetDestination(_StarterPoint.position);
    }

    bool RandomSearchPoint(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius; 

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    void SetRandomPatrolPoint()
    {
        _enemyAgent.SetDestination(_patrolPoints[Random.Range(0,_patrolPoints.Length)].position);
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position,_player.position) < _detectionRange)
        {
            return true;
        }
        else
        {
            return false;
        }*/
        Vector3 directionToPlayer = _player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if(_player.position == _playerLastPositionKnown)
        {
            return true;
        }

        if(distanceToPlayer > _detectionRange)
        {
            return false;
        }
        if(angleToPlayer > _detectionAngle * 0.5f)
        {
            return false;
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if(hit.collider.CompareTag("Player"))
            {
                _playerLastPositionKnown = _player.position;
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach(Transform point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position,0.5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.yellow;

        Vector3 fovLine1 = Quaternion.AngleAxis(_detectionAngle * 0.5f, transform.up) * transform.forward * _detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-_detectionAngle * 0.5f, transform.up) * transform.forward * _detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
         Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
