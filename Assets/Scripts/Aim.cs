using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Aim : MonoBehaviour
{
    public Vector2 desiredLook;
    public float rotationSpeed;

    private Transform _turret;
    private float _rotationY;

    void OnValidate()
    {
        _turret = GetComponent<Transform>();
    }

    private void Start()
    {
        //Soluciona que el centro de masas no está centrado en el rigidbody
        //_rigidbody.centerOfMass = Vector3.zero;
        //_rigidbody.inertiaTensorRotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        //turret.SetParent(null);
        // GIRAR LA TORRETA  (IZQUIERDA Y DERECHA)
        /*
        _rotationY += desiredLook.x * rotationSpeed * Time.fixedDeltaTime;
        Debug.Log("desiredLook " + desiredLook.x);
        Debug.Log("rotationY " + _rotationY);
        Debug.Log("rotationSpeed " + rotationSpeed);

        _turret.rotation = Quaternion.Euler(
            _turret.rotation.eulerAngles.x,
            _rotationY,
            _turret.rotation.eulerAngles.z);
        */

        //transform.RotateAround

        moveMouse();
    }

    private void moveMouse()
    {
        // Crear un rayo desde la posición de la cámara hasta el puntero del mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Detectar si el rayo golpea algo en el mundo
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(_turret.position, hit.point, Color.red);
            // Obtener la posición del puntero en el mundo
            Vector3 mousePosition = hit.point;

            // Hacer que el objeto mire hacia la posición del puntero del mouse
            _turret.LookAt(mousePosition, Vector3.up);

            //mousePosition.y = 0;
            //Quaternion rotation = Quaternion.LookRotation(mousePosition, Vector3.up);
            //turret.rotation = rotation;
        }
    }
}
