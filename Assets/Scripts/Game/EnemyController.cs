using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [Range(0f, 5f)]
    public float movementSpeed = 3;
    [Range(0f, 100f)]
    public float rotationSpeed = 80;
    [Range(0.1f, 1f)]
    public float precisionRotate = 0.97f;

    [Header("Fire")]
    [Range(0.1f, 10f)]
    public float cooldownFireRate = 3;
    [Range(0.1f, 1f)]
    public float precisionFire = 0.97f;
    [Range(0.1f, 1f)]
    public float turretRotationSpeed = 0.7f;
    [Range(0f, 50f)]
    public float wallDistance = 20f;
    [Range(0f, 100f)]
    public float enemyDistance = 50f;

    public enum SelectedTraking
    {
        FindClosestPlayer,
        FocusTarget
    }
    [Header("Type Of Traking")]
    [Range(1f, 5f)]
    public float coolDownTime = 3f;
    [SerializeField]
    public SelectedTraking traking;

    private Animator _animator;
    private Weapon _weapon;

    private Vector2 _desiredMovement;
    private Vector3 _initialPosition;
    private Vector3 _towardsDirection;
    private NavMeshPath _path;
    private bool _canFire = true;
    private GameObject goTarget;

    //Maq Estados
    public enum State
    {
        Idle,
        TrackingTarget,
        Cooldown,
        Move,
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
        _weapon = gameObject.GetComponentInChildren<Weapon>();

        _initialPosition = gameObject.transform.position;        

        //Maq.Estados-----------
        ChangeState(State.TrackingTarget);
        StartCoroutine(FSM());
    }

    //Maq.Estados------------
    void ChangeState(State nextState)
    {
        _currentState = nextState;
    }

    IEnumerator FSM()
    {
        while (true)
        {   //Se lanza el estado y estaremos en �l hasta que salgamos
            yield return StartCoroutine(_currentState.ToString());
        }
    }
    IEnumerator Idle()
    {
        //Punto de entrada
        //Debug.Log("Entrando en Idle...");

        //Ejecuci�n del estado
        while (_currentState == State.Idle)
        {
            if (_currentTarget != null)
                ChangeState(State.TrackingTarget);

            yield return 0;
        }

        //Punto de salida
        //Debug.Log("Saliendo de Idle...");
    }
    IEnumerator TrackingTarget()
    {
        //Punto de entrada
        //Debug.Log("Entrando en Tracking...");

        if (traking == SelectedTraking.FindClosestPlayer)
        {
            // Busca todo el rato el target m�s cercano
            goTarget = FindClosestPlayer();
        }else if(traking == SelectedTraking.FocusTarget)
        {
            // FIJA un Taget
            goTarget = FocusTarget();
        }        

        //Ejecuci�n del estado
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
                //MOVIMIENTO
                //ChangeState(State.Move);
                if (_currentTarget != null && _currentTarget.position != null)
                {
                    //_navMeshAgent.SetDestination(_currentTarget.position);
                    _desiredMovement = new Vector2(_currentTarget.position.x, _currentTarget.position.z);
                    //_movement._desiredMovement = _desiredMovement;

                    RotateToMesh(); //Rota

                    float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
                    if (distance > 5)   //Si no est� pegado al target
                    {
                        MoveForward();  //Avanza
                    }

                }
                else
                    _navMeshAgent.SetDestination(_initialPosition);

                //MOVER TORRETA
                //ChangeState(State.Aim);
                Vector3 towardsTarget = _currentTarget.position - transform.position;
                _weapon.transform.forward =
                    Vector3.Lerp(_weapon.transform.forward, towardsTarget.normalized, Time.deltaTime * turretRotationSpeed);
                //_weapon.transform.LookAt(_currentTarget);   //Instant�neo

                //DISPARA
                bool canShoot = Vector3.Dot(_weapon.transform.forward, towardsTarget.normalized) > precisionFire;
                if (canShoot && _canFire)
                {//-----Lanzar un rayo para comprobar si hay un muro muy cerca-----
                    if (CheckFreeShoot(towardsTarget))
                    {
                        //Debug.Log("DISPAROOO!!");
                        _weapon.Fire();
                        StartCoroutine(CooldownFireRate());
                    }                       
                }
            }
            else
                ChangeState(State.Cooldown);

            yield return 0;
        }

        //Punto de salida
        //Debug.Log("Saliendo de Tracking...");
    }
    /*
    IEnumerator Move()
    {
        //Punto de entrada
        Debug.Log("Entrando en Move...");

        //Ejecuci�n del estado
        while (_currentState == State.Move)
        {
            //Movimiento
            if (_currentTarget != null && _currentTarget.position != null)
            {
                //_navMeshAgent.SetDestination(_currentTarget.position);
                _desiredMovement = new Vector2(_currentTarget.position.x, _currentTarget.position.z);
                //_movement._desiredMovement = _desiredMovement;

                RotateToMesh(); //Rota

                float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
                if (distance > 5)   //Si no est� pegado al target
                {
                    MoveForward();  //Avanza
                }

            }
            else
                _navMeshAgent.SetDestination(_initialPosition);

            ChangeState(State.TrackingTarget);

            yield return 0;
        }

        //Punto de salida
        Debug.Log("Saliendo de Move...");
    }*/
    /*
    IEnumerator Aim()
    {
        //Punto de entrada
        Debug.Log("Entrando en Aim...");

        //Ejecuci�n del estado
        while (_currentState == State.Aim)
        {
            

            ChangeState(State.TrackingTarget);

            yield return 0;
        }

        //Punto de salida
        Debug.Log("Saliendo de Aim...");
    }*/
    IEnumerator Cooldown()
    {
        //Punto de entrada
        //Debug.Log("Entrando en Cooldown...");
        float elapsedTime = 0;

        _animator.SetBool("Forward", false);
        _animator.SetBool("Right", false);
        _animator.SetBool("Left", false);

        //Ejecuci�n del estado
        while (elapsedTime < coolDownTime)
        {
            _navMeshAgent.velocity = Vector3.zero;  //Para el tanque
            //Giro de la torreta a modo de radar
            _weapon.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }

        //Punto de salida
        //Debug.Log("Saliendo de Cooldown...");
        //ChangeState(State.Idle);
        ChangeState(State.TrackingTarget);
    }
    //----------------Maq.Estados

    IEnumerator CooldownFireRate()
    {
        //Punto de entrada
        //Debug.Log("Entrando en FireRate...");

        //Ejecuci�n del estado
        _canFire = false;
        yield return new WaitForSeconds(cooldownFireRate);
        _canFire = true;

        //Punto de salida
        //Debug.Log("Saliendo de FireRate...");
    }
    private bool CheckFreeShoot(Vector3 towardsTarget)
    {
        bool freeShoot = false;
        bool freeWall = true;
        bool freeEnemy = true;
        RaycastHit hitWall;
        RaycastHit hitEnemy;
        if (Physics.Raycast(_weapon.transform.position, towardsTarget, out hitWall, wallDistance)) // Comprueba si choca
        {
            //Debug.DrawLine(_weapon.transform.position, hitWall.point, Color.green);
            // Comprueba si la colisi�n es con una pared
            if (hitWall.collider.GetComponentInParent<Wall>() != null)
            {
                freeWall = false;
            }
        }
        if (Physics.Raycast(_weapon.transform.position, towardsTarget, out hitEnemy, enemyDistance)) // Comprueba si choca
        {
            //Debug.DrawLine(_weapon.transform.position, hitEnemy.point, Color.blue);
            // Comprueba si la colisi�n es con un enemigo "Aliado"
            if (hitEnemy.collider.GetComponent<EnemyController>() != null)
            {
                freeEnemy = false;
            }
        }

        if (freeWall == true && freeEnemy == true)
            freeShoot = true;

        return freeShoot;
    }
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
            //Debug.Log($"Objetivo {closestPlayer.name}");
        }
        return closestPlayer;
    }
    private GameObject FocusTarget()
    {
        GameObject goFocus = null;
        //Si no tiene target, el enemigo busca al target (si existe)
        if (_currentTarget == null)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                goFocus = GameObject.FindGameObjectWithTag("Player");
                //Debug.Log("Fijando Objetivo...");
                //Debug.Log($"Objetivo {goFocus.name}");
            }
        }
        return goFocus;
    }
    private void RotateToMesh()
    {
        _path = new NavMeshPath();
        _animator.SetBool("Forward", false);
        if (_navMeshAgent.CalculatePath(_currentTarget.position, _path))
        {
            if (_path.corners.Length > 1)
            {
                //Direcci�n a seguir en la malla
                _towardsDirection = (_path.corners[1] - transform.position).normalized;
                //transform.rotation = Quaternion.LookRotation(direction);

                //ROTAR
                Quaternion targetRotation = Quaternion.LookRotation(_towardsDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


                //Compruebo si el tanque est� rotando a la derecha o a la izquierda
                float dotRight = Vector3.Dot(_navMeshAgent.transform.right, _towardsDirection); // + derecha | - izquierda
                if (dotRight > 0)
                {
                    //Animaci�n de rotaci�n a la derecha
                    _animator.SetBool("Right", true);
                    _animator.SetBool("Left", false);
                }
                else
                {
                    //Animaci�n de rotaci�n a la izquierda
                    _animator.SetBool("Right", false);
                    _animator.SetBool("Left", true);
                }
            }
        }
        //Paro las animaciones
        _animator.SetBool("Forward", false);
        _animator.SetBool("Left", false);
        _animator.SetBool("Right", false);
    }
    private void MoveForward()
    {
        //Si player est� dentro del mapa va a por �l, sino lo "busca"
        if (_path.corners.Length > 0)
            _towardsDirection = (_path.corners[1] - transform.position).normalized;
        else
            ChangeState(State.Cooldown);

        _animator.SetBool("Right", false);
        _animator.SetBool("Left", false);

        //AVANZA si est� en el forward
        if (Vector3.Dot(_navMeshAgent.transform.forward, _towardsDirection) > precisionRotate)
        {
            _navMeshAgent.Move(transform.forward * movementSpeed * Time.deltaTime);

            //Animaci�n Forward     (todo lo de abajo es para controlar la animaci�n)
            _animator.SetBool("Forward", true);
        }
        else
        {
            _navMeshAgent.speed = 0f;
        }
    }
}
