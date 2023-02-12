using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public Weapon weapon;
    public Transform turret;
    public Vector2 desiredMovement;

    public float movementSpeed = 3;
    public float rotationSpeed = 10;
    public float precisionRotate = 9.7f;

    private Animator _animator;

    private Vector3 _initialPosition;
    private Vector3 _towardsDirection;
    private NavMeshPath _path;

    //Maq Estados
    public enum State
    {
        Idle,
        TrackingTarget,
        Cooldown,
        Moving,
        Aim,
        Shoot,
        ReturnHome
    }
    State _currentState;

    //IA
    private NavMeshAgent _navMeshAgent;
    private Transform _currentTarget;
    /*
    private void OnValidate()
    {
        _movement = GetComponent<Movement>();
    }*/

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _initialPosition = gameObject.transform.position;

        //Maq.Estados-----------
        ChangeState(State.TrackingTarget);
        StartCoroutine(FSM());
    }

    //Mac.Estados------------
    void ChangeState(State nextState)
    {
        _currentState = nextState;
    }

    IEnumerator FSM()
    {
        while (true)
        {   //Se lanza el estado y estaremos en él hasta que salgamos
            yield return StartCoroutine(_currentState.ToString());
        }
    }
    IEnumerator Idle()
    {
        //Punto de entrada
        Debug.Log("Entrando en Idle...");

        //Ejecución del estado
        while (_currentState == State.Idle)
        {
            if (_currentTarget != null)
                ChangeState(State.TrackingTarget);

            yield return 0;
        }

        //Punto de salida
        Debug.Log("Saliendo de Idle...");
    }
    IEnumerator TrackingTarget()
    {
        //Punto de entrada
        Debug.Log("Entrando en Tracking...");

        // Busca todo el rato el target más cercano
        //GameObject goTarget = FindClosestPlayer();
        // FIJA un Taget
        GameObject goTarget = Focustarget();

        //Ejecución del estado
        while (_currentState == State.TrackingTarget)
        {
            if (goTarget != null)
            {
                _currentTarget = goTarget.transform;
                //Debug.Log($"Objetivo --> {_currentTarget.name}");
            }

            bool hasTarget = _currentTarget != null;
            if (hasTarget)
            {
                //Movimiento
                if (_currentTarget != null && _currentTarget.position != null)
                {
                    //_navMeshAgent.SetDestination(_currentTarget.position);
                    desiredMovement = new Vector2(_currentTarget.position.x, _currentTarget.position.z);
                    //_movement.desiredMovement = desiredMovement;

                    RotateToMesh(); //Rota
                    MoveForward();  //Avanza
                }

                else
                    _navMeshAgent.SetDestination(_initialPosition);

                /*DISPARA
                bool canShoot = Vector3.Dot(_weapon.transform.forward, towardsTarget.normalized) > 0.99;
                if (canShoot)
                    _weapon.Fire();
                */
            }
            else
                ChangeState(State.Cooldown);

            yield return 0;
        }

        //Punto de salida
        Debug.Log("Saliendo de Tracking...");
    }
    IEnumerator Cooldown()
    {
        //Punto de entrada
        Debug.Log("Entrando en Cooldown...");
        float coolDownTime = 5f;
        float elapsedTime = 0;
        //Vector3 restDirection = _weapon.transform.forward + Vector3.down * 2;

        //Ejecución del estado
        while (elapsedTime < coolDownTime)
        {
            _navMeshAgent.velocity = Vector3.zero;
            //PointTowards(restDirection);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }

        //Punto de salida
        Debug.Log("Saliendo de Cooldown...");
        //ChangeState(State.Idle);
        ChangeState(State.TrackingTarget);
    }
    //----------------Mac.Estados

    private GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = float.MaxValue;
        GameObject closestPlayer = null;
        //Debug.Log("Buscando Objetivos...");
        if (players.Length > 0)
        {
            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }
            Debug.Log($"Objetivo {closestPlayer.name}");
        }
        return closestPlayer;
    }
    private GameObject Focustarget()
    {
        GameObject goFocus = null;
        //Si no tiene target, el enemigo busca al target (si existe)
        if (_currentTarget == null)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                goFocus = GameObject.FindGameObjectWithTag("Player");
                Debug.Log("Fijando Objetivo...");
                Debug.Log($"Objetivo {goFocus.name}");
            }
        }
        return goFocus;
    }
    private void RotateToMesh()
    {
        _path = new NavMeshPath();
        Vector3 direction;
        if (_navMeshAgent.CalculatePath(_currentTarget.position, _path))
        {
            if (_path.corners.Length > 1)
            {
                //Dirección a seguir en la malla
                direction = (_path.corners[1] - transform.position).normalized;
                //transform.rotation = Quaternion.LookRotation(direction);

                //ROTAR
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    private void MoveForward()
    {
        _towardsDirection = (_path.corners[1] - transform.position).normalized;

        //AVANZA si está en el forward
        if (Vector3.Dot(_navMeshAgent.transform.forward, _towardsDirection) > 0.95f)
        {
            _navMeshAgent.Move(transform.forward * movementSpeed * Time.deltaTime);

            //Animación Forward     (todo lo de abajo es para controlar la animación)
            _animator.SetBool("Forward", true);
            _animator.SetBool("Right", false);
            _animator.SetBool("Left", false);
        }
        else
        {
            _navMeshAgent.speed = 0f;

            //Compruebo si el tanque está rotando a la derecha o a la izquierda
            float dotRight = Vector3.Dot(_navMeshAgent.transform.right, _towardsDirection); // + derecha | - izquierda
            if (dotRight > 0)
            {
                //Animación de rotación a la derecha
                _animator.SetBool("Right", true);
                _animator.SetBool("Left", false);
            }
            else
            {
                //Animación de rotación a la izquierda
                _animator.SetBool("Right", false);
                _animator.SetBool("Left", true);
            }
        }
    }
    /*
    private void Update()
    {
        //Si no tiene target, el enemigo busca al target (si existe)
        if (_currentTarget == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            //_player = GetComponent<Player>();
            if (go != null)
            {
                _currentTarget = go.transform;
                //Debug.Log($"target --> {_target}");
            }
        }

        if (_currentTarget != null && _currentTarget.position != null)
            _navMeshAgent.SetDestination(_currentTarget.position);
        else
            _navMeshAgent.SetDestination(_initialPosition);

    }
    */

    /*
    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector2 movement = value.Get<Vector2>();
        Vector2 movement = new Vector2(20,20);
        _movement.desiredMovement = movement;

        if (weapon != null)
        {
            //weapon.Fire();
            //Debug.Log("PewPew");
        }
    }*/

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;
        Destroy(gameObject);
    }
}
