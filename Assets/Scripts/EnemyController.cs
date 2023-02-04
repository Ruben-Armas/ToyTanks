using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Weapon weapon;

    [SerializeField]
    private Movement _movement;

    private void OnValidate()
    {
        _movement = GetComponent<Movement>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

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
    }

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;
        Destroy(gameObject);
    }
}
