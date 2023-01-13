using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public float maxSpeed;
    public Vector2 desiredMovement;

    private Rigidbody _rigidbody;
    private float _rotationY;

    //También funcionaría con Awake, pero puede hacer que al inicio de la partida se para un momento mientras se configura todo
    void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // MOVIMIENTO DEL PERSONAJE
        /* Mueve hacia delante, no al forward del objeto
            //Para convertir a Vector2
            Vector3 velocity = new Vector3(desiredMovement.x, 0, desiredMovement.y);
            _rigidbody.velocity = velocity * (maxSpeed * Time.fixedDeltaTime);*/

        // Movimiento hacia delante (forward)
        Vector3 forwardVelocity = _rigidbody.transform.forward * desiredMovement.y;
        // Movimiento lateral
        Vector3 strafeVelocity = _rigidbody.transform.right * desiredMovement.x;
        // Sumamos los vectores (normalizados para que no vaya más rápido en diagonal)
        //  -> dará el vector|dirección resultante
        _rigidbody.velocity = (forwardVelocity + strafeVelocity).normalized * (maxSpeed * Time.fixedDeltaTime);

    }
}