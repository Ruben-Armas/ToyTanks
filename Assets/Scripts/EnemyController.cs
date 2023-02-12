using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //public Weapon weapon;

    //[SerializeField]
    //private Movement _movement;
    //private Player _player;

    private Vector3 _initialPosition;

    //IA
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    /*
    private void OnValidate()
    {
        _movement = GetComponent<Movement>();
    }*/

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        //  Debug.Log($"_navMeshAgent -- {_navMeshAgent}");

        _initialPosition = gameObject.transform.position;
    }

    private void Update()
    {
        //El enemigo busca al target (si existe)
        if (_target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            //_player = GetComponent<Player>();
            if (go != null)
            {
                _target = go.transform;
                //Debug.Log($"target --> {_target}");
            }
        }

        if (_target != null && _target.position != null)
            _navMeshAgent.SetDestination(_target.position);
        else
            _navMeshAgent.SetDestination(_initialPosition);

    }
    

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
