using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Aim))]
public class PlayerInputHandler : MonoBehaviour
{
    public float turnSensitivity;
    //public Weapon weapon;

    private Movement _movement;
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
    /*
    public void OnFire(InputValue value)
    {
        if (weapon != null)
        {
            weapon.Fire();
            Debug.Log("PewPew");
        }
    }
    */
    public void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _movement.desiredMovement = movement;
        //Debug.Log($"Walk{movement}");
    }

    public void OnLook(InputValue value)
    {
        Vector2 look = value.Get<Vector2>();
        _aim.desiredLook = look * turnSensitivity;     // Mira derecha|izquierda
        //Debug.Log($"Look{look}");
    }
}