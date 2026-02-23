using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemigoScript : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;
    private Transform _player;
    
    [SerializeField] private float _detectionRange = 7f;
    [SerializeField] private float _attackRange = 4f;
    [SerializeField] private Transform[] _patrolPoints;
    public EnemyState currentState;

    float attackTimer;
    float attackDelay = 2;

    #region Awake
    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();

        _player = GameObject.FindWithTag("Player").transform;
    }
    public enum EnemyState
    {
        Patrolling,

        Chasing,

        Searching,

        Waiting,

        Attacking
    }

    #endregion
    #region Start

    void Start()
    {
        currentState = EnemyState.Patrolling;
        PatrolInOrder();
    }
    
    #endregion
    #region Update
    
    void Update()
    {
        switch(currentState)
        {
            case EnemyState.Patrolling:

                Patrol();

            break;

            case EnemyState.Chasing:

                Chasing();

            break;

            case EnemyState.Attacking:

                
                Attacking();
                
            break;

            default:

                Patrol();

            break;
        }
    }
    
    #endregion
    #region OnRange
    
    /*bool OnRange() //funcion oficial para examen OnRange
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position); //crea un vector que te dice a que distancia esta el jugador
        if(distanceToPlayer <= _detectionRange) //comprueba la distancia entre el player y el enemigo
        {
            return true; //devuelve un valor true
        }
        else
        {
            return false; //devuelve un valor false
        }
    }*/

    //version 2 del on range un poco mas elavorado
    bool OnRange(float distance) //funcion oficial para examen OnRange
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position); 
        if(distanceToPlayer <= distance) 
        {
            return true; 
        }
        else
        {
            return false; 
        }
    }
    
    #endregion
    #region Patrol
    
    void Patrol() //funcion oficial para examen Patrol
    {
        if(OnRange()) //necesario para que te detecte en todos los estados
        {
            currentState = EnemyState.Chasing; //cambia de estado a chasing
        }
        if(_enemyAgent.remainingDistance < 0.5f) //detecta si ha llegado a un punto y le asigna el siguiente
        {
            PatrolInOrder(); //llama la funcion PatrolInOrder()
        }
    }

    void PatrolInOrder()
    {
        _enemyAgent.SetDestination(_patrolPoints[_patrolIndex].position); //le asigna el siguiente punto al enemigo
        _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Lenght; //le suma 1 al controlador de indices para que el siguiente no sea el mismo
    }
    
    #endregion
    #region Chasing

    void Chasing()
    {
        if(!OnRange(_detectionRange)) //comprueba si aun estas en rango de deteccion
        {
            currentState = EnemyState.Patrolling; //cambia el estado de patrolling en caso que ya no estes en rango
        }
        if(!OnRange(_attackRange))
        {
            attackTimer = attackDelay;
            currentState = EnemyState.Patrolling; 
        }
        _enemyAgent.SetDestination(_player.position);
    }

    #endregion
    #region Attack

    void Attack()
    {
        if(!OnRange(_attackRange)) 
        {
            currentState = EnemyState.Chasing;
        }

        if(attackTimer < attackDelay)
        {
            attackTimer += Time.deltaTime;

            return;
        }

        Debug.Log("Attack");
        attackTimer = 0;
    }
    
    #endregion
}
