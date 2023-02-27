using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour
{
    [Header("Movimiento")]
    [Range(0f, 350f)]
    public float maxSpeed = 250;
    [Range(0f, 200f)]
    public float rotationSpeed = 150;
    [Range(0f, 1f)]
    public float precisionRotate = 0.8f;
    public Vector2 desiredMovement;

    private Rigidbody _rigidbody;
    private float _rotationY;
    private Quaternion _lastRotation;

    private Animator _animator;

    //También funcionaría con Awake, pero puede hacer que al inicio de la partida se para un momento mientras se configura todo
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //Soluciona que el centro de masas no está centrado en el rigidbody
        _rigidbody.centerOfMass = Vector3.zero;
        _rigidbody.inertiaTensorRotation = Quaternion.identity;
    }


    
    private void FixedUpdate()
    {
        //--MOVIMIENTO DEL PERSONAJE--
        //Mueve según el mundo, no al forward del objeto            
        Vector3 velocity = new Vector3(desiredMovement.x, 0, desiredMovement.y);    //Para convertir a Vector2
        Vector3 vel = velocity.normalized * (maxSpeed * Time.fixedDeltaTime);

        //Debug.Log($"Vel {vel}");

        //Rota el tanque al forward de la dirección del movimiento
        //Vector3 targetOrientation = _rigidbody.velocity.normalized;

        //Solo rota cuando te mueves y no se reinicia la rotación
        if (vel.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(vel);
            //--ROTA EN LA DIRECCIÓN DE LA VELOCIDAD--
            _rigidbody.rotation = Quaternion.RotateTowards(
                _rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //--Muevo si - (rotación)el forward y la dirección a la que se mueve(velocidad) están alineados - dentro de un rango 
            float dot = Vector3.Dot(transform.forward, velocity.normalized);    // 1 forward y velocidad alineada | 0 perpendicular
            float dotRight = Vector3.Dot(transform.right, velocity.normalized); // + derecha | - izquierda
            //Debug.Log($"dotR --> {dotRight}");
            if (dot > 0.9f)
            {
                _rigidbody.velocity = vel;  //--MUEVE--

                //Animación Forward     (todo lo de abajo es para controlar la animación)
                _animator.SetBool("Forward", true);
                _animator.SetBool("Right", false);
                _animator.SetBool("Left", false);
            }
            else
            {
                //yaw (eje y) -> valor de la rotación actual
                //float yaw = _rigidbody.rotation.eulerAngles.y;
                //Debug.Log($"yaw   -> {yaw}");

                //Compruebo si el tanque está rotando a la derecha o a la izquierda
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
        else
        {
            //Paro las animaciones
            _animator.SetBool("Forward", false);
            _animator.SetBool("Left", false);
            _animator.SetBool("Right", false);
        }

        //ROTA Instantáneo
        //_rigidbody.rotation = Quaternion.LookRotation(velocity);


        /*
        // Movimiento hacia delante (forward)
        Vector3 forwardVelocity = _rigidbody.transform.forward * desiredMovement.y;
        // Movimiento lateral
        Vector3 strafeVelocity = _rigidbody.transform.right * desiredMovement.x;
        // Sumamos los vectores (normalizados para que no vaya más rápido en diagonal)
        //  -> dará el vector|dirección resultante
        _rigidbody.velocity = (forwardVelocity + strafeVelocity).normalized * (maxSpeed * Time.fixedDeltaTime);
       */
    }
}