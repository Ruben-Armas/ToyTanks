using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
//[RequireComponent(typeof(Aim))]
public class PlayerInputHandler : MonoBehaviour
{
    public float turnSensitivity;
    public Weapon weapon;
    public Transform turret;
    public float lookSpeed;
    public Vector3 desiredLook;

    [SerializeField]
    private Movement _movement;
    [SerializeField]
    private Aim _aim;

    private void OnValidate()
    {
        _movement = GetComponent<Movement>();
        _aim = GetComponent<Aim>();
    }
    /*
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    */
    
    public void OnFire(InputValue value)
    {
        if (weapon != null)
        {
            weapon.Fire();
            //Debug.Log("PewPew");
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _movement.desiredMovement = movement;
        //Debug.Log($"Walk{movement}");
    }

    public void OnLookMouse(InputValue value)
    {
        Vector2 mousePosition = value.Get<Vector2>();
        //Punto del ratón en el mundo
        Vector2 tankScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //Vector dirección (diferencia de dónde estoy y donde está el punto al que miro)
        Vector2 towardsMouse = (mousePosition - tankScreenPosition).normalized;
        //Apunta el forward en los ejes X Z
        turret.forward = new Vector3 (towardsMouse.x, 0, towardsMouse.y);     // Mira derecha|izquierda
        //Debug.Log($"Look{mousePosition}");
    }

    public void OnLookJoystick(InputValue value)
    {
        Vector2 joystickLook = value.Get<Vector2>();

        //Solo rota cuando mueves el joystick y no se reinicia la rotación
        if (joystickLook.magnitude != 0)
        {
            turret.forward = new Vector3(joystickLook.x, 0, joystickLook.y);     // Mira derecha|izquierda


            /*//GRADUAL??
            desiredLook = new Vector3(joystickLook.x, 0, joystickLook.y);

            //turret.forward = Vector3.Lerp(turret.forward, desiredLook, lookSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(desiredLook, Vector3.up);

            //turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, lookSpeed * Time.deltaTime);
            turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, Time.deltaTime * lookSpeed);

            Debug.Log($"Tr{turret.rotation}");
            */


            /*-------
            Debug.Log($"Look{joystickLook}");
            Debug.Log($"x{joystickLook.x} y{joystickLook.y}");

            desiredLook = new Vector3(joystickLook.x, 0, joystickLook.y);
            Debug.Log($"dLook{desiredLook}");
            Quaternion targetRotation = Quaternion.LookRotation(desiredLook);
            turret.rotation = targetRotation;
            //turret.rotation = Quaternion.Lerp(turret.rotation.normalized, targetRotation.normalized, lookSpeed * Time.deltaTime);
            Debug.Log($"tR{targetRotation}");
            //turret.forward = Vector3.Lerp(turret.forward, desiredLook, lookSpeed * Time.deltaTime);
            ----*/


            /*
            //_rotationY += desiredLook.x * rotationSpeed * Time.fixedDeltaTime;
            turret.rotation = Quaternion.Euler(
                turret.rotation.eulerAngles.x,
                joystickLook.x * lookSpeed * Time.fixedDeltaTime,
                turret.rotation.eulerAngles.z);
            */
        }

        
    }}