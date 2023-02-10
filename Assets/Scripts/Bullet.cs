using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public Rigidbody rigidbody;
    public int numBounces;
    private int contBounces;
    private bool canDetectCollision = true;

    private EnemyController enemy;
    private Player player;
    private Bullet bullet;
    private Shield currentShield;


    /*Si lo pongo en el FixedUpdate estar�a siempre aplicando esa velocidad
     * por lo que no caer�a por la gravedad
     * Pero si lo hago en el Start() -> tendremos balas con caida
    */
    public void Start()
    {//No hace falta el deltaTime porque solo se usa cuando "se suma"
        rigidbody.velocity = transform.forward * speed;
        contBounces = 0;
    }

    private void FixedUpdate()
    {
        //Debug.DrawLine(rigidbody.velocity, transform.forward, Color.black);
       
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rigidbody.velocity, out hit, 1)) // Comprueba si choca
        {
            // Comprueba si la colisi�n es con una pared
            if (hit.collider.GetComponentInParent<Wall>() != null)
            {
                Debug.Log("Wall");
                if (checkIfSelfDestroy() == false)
                {
                    // Calcula la normal de la superficie de la pared con la que chocando
                    Vector3 normal = hit.normal;
                    Debug.Log($"contact hit normal -> {normal}");

                    // Calcula la direcci�n de rebote
                    Vector3 reflectedDirection = Vector3.Reflect(rigidbody.velocity, hit.normal);
                    //Debug.Log($"reflectedDirection -> {reflectedDirection}");
                    Debug.DrawLine(hit.point, hit.point + normal * 10, Color.red, 5);
                    Debug.DrawLine(hit.point, hit.point + reflectedDirection * 10, Color.blue, 5);

                    // Gira a la nueva direcci�n
                    transform.forward = reflectedDirection;

                    //Reinicio la inercia
                    //rigidbody.velocity = Vector3.zero;

                    // Mueve la bala
                    rigidbody.velocity = transform.forward * speed;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si la colisi�n es con un jugador, un enemigo o una bala
        enemy = collision.collider.GetComponentInParent<EnemyController>();
        player = collision.collider.GetComponentInParent<Player>();
        bullet = collision.collider.GetComponentInParent<Bullet>();

        if (enemy != null)
        {
            Debug.Log("Contact ENEMY");
            enemy.SetDestroyed();   //Destruye al impactado
            Destroy(gameObject);    //La bala se autodestruye
        }

        if (player != null)
        {
            //Debug.Log("Contact PLAYER");
            currentShield = player.GetComponent<Shield>();
            bool haveShield = currentShield.getShield();
            Debug.Log(haveShield);
            if (currentShield != null && currentShield.getShield())   //Si tiene escudo
            {
                currentShield.deactivateShield();   //Lo desactivo           
                Debug.Log("Escudo Rotooo!!");
            }
            else   
                player.SetDestroyed();  //Destruye al impactado

            Destroy(gameObject);    //La bala se autodestruye
        }

        if (bullet != null)
        {
            Debug.Log("Contact BULLET");
            bullet.SetDestroyed();   //Destruye al impactado
            Destroy(gameObject);    //La bala se autodestruye
        }
    }


    private bool checkIfSelfDestroy()
    {
        contBounces++;
        if (contBounces >= numBounces)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;
        Destroy(gameObject);
    }
}

//LA BALA EMPUJA AL TANQUE CUANDO SE ROMPE EL ESCUDO    �arreglarlo?