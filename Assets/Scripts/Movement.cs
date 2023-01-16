using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public float maxSpeed;
    public float rotationSpeed;
    public Vector2 desiredMovement;

    private Rigidbody _rigidbody;
    private float _rotationY;
    private Quaternion _lastRotation;

    //Tambi�n funcionar�a con Awake, pero puede hacer que al inicio de la partida se para un momento mientras se configura todo
    void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Soluciona que el centro de masas no est� centrado en el rigidbody
        _rigidbody.centerOfMass = Vector3.zero;
        _rigidbody.inertiaTensorRotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        // MOVIMIENTO DEL PERSONAJE
        //Mueve hacia delante, no al forward del objeto            
        Vector3 velocity = new Vector3(desiredMovement.x, 0, desiredMovement.y);    //Para convertir a Vector2
        Vector3 vel = velocity.normalized * (maxSpeed * Time.fixedDeltaTime);
        _rigidbody.velocity = vel;

        
        //Rota el tanque al forward de la direcci�n del movimiento
        //Vector3 targetOrientation = _rigidbody.velocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(vel);

        Debug.Log("vel -->" + vel.normalized);
        Debug.Log("rot -->" + _rigidbody.rotation);

        //Solo rota cuando te mueves
        if (vel.magnitude != 0)
        {
            _rigidbody.rotation = Quaternion.RotateTowards(
            _rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            if (_rigidbody.rotation.Equals(vel))
            {
                _rigidbody.velocity = vel;
            }
        }
        
        //Instant�neo
        //_rigidbody.rotation = Quaternion.LookRotation(velocity);


        /*
               // Movimiento hacia delante (forward)
               Vector3 forwardVelocity = _rigidbody.transform.forward * desiredMovement.y;
               // Movimiento lateral
               Vector3 strafeVelocity = _rigidbody.transform.right * desiredMovement.x;
               // Sumamos los vectores (normalizados para que no vaya m�s r�pido en diagonal)
               //  -> dar� el vector|direcci�n resultante
               _rigidbody.velocity = (forwardVelocity + strafeVelocity).normalized * (maxSpeed * Time.fixedDeltaTime);
       */
    }
}