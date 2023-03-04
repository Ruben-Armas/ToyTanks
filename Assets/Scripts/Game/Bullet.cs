using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> activar sonido dispararse
    public delegate void BulletSFXShoot();
    public static event BulletSFXShoot onBulletSFXShoot;  //(EVENTO)
    //EVENTO (DELEGADO)   --> activar sonido al rebotar
    public delegate void BulletSFXBounce();
    public static event BulletSFXBounce onBulletSFXBounce;  //(EVENTO)
    //EVENTO (DELEGADO)   --> activar sonido al rebotar
    //public delegate void BulletSFXDestroy();
    //public static event BulletSFXDestroy onBulletSFXDestroy;  //(EVENTO)

    public float speed;

    public Rigidbody rigidbody;
    public int numBounces;
    private int contBounces;

    private Enemy enemy;
    private Player player;
    private Bullet bullet;
    private Shield currentShield;


    /*Si lo pongo en el FixedUpdate estaría siempre aplicando esa velocidad
     * por lo que no caería por la gravedad
     * Pero si lo hago en el Start() -> tendremos balas con caida
    */
    public void Start()
    {//No hace falta el deltaTime porque solo se usa cuando "se suma"
        rigidbody.velocity = transform.forward * speed;
        contBounces = 0;

        //Evento Sonido Disparo
        if (onBulletSFXShoot != null)
            onBulletSFXShoot();
    }

    private void FixedUpdate()
    {
        //Debug.DrawLine(rigidbody.velocity, transform.forward, Color.black);
       
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rigidbody.velocity, out hit, 1)) // Comprueba si choca
        {
            // Comprueba si la colisión es con una pared
            if (hit.collider.GetComponentInParent<Wall>() != null || hit.collider.GetComponent<Wall>() != null)
            {
                //Debug.Log("Wall");
                if (checkIfSelfDestroy() == false)
                {
                    //Evento Sonido Rebote
                    if (onBulletSFXBounce != null)
                        onBulletSFXBounce();

                    // Calcula la normal de la superficie de la pared con la que chocando
                    Vector3 normal = hit.normal;
                    //Debug.Log($"contact hit normal -> {normal}");

                    // Calcula la dirección de rebote
                    Vector3 reflectedDirection = Vector3.Reflect(rigidbody.velocity, hit.normal);
                    //Debug.Log($"reflectedDirection -> {reflectedDirection}");
                    Debug.DrawLine(hit.point, hit.point + normal * 10, Color.red, 5);
                    Debug.DrawLine(hit.point, hit.point + reflectedDirection * 10, Color.blue, 5);

                    // Gira a la nueva dirección
                    transform.forward = reflectedDirection;

                    //Reinicio la inercia
                    //rigidbody.velocity = Vector3.zero;

                    // Mueve la bala
                    rigidbody.velocity = transform.forward * speed;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    //private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si la colisión es con un jugador, un enemigo o una bala
        enemy = other.GetComponentInParent<Enemy>();
        player = other.GetComponent<Player>();
        bullet = other.GetComponentInParent<Bullet>();

        if (enemy != null)
        {
            //Debug.Log("Contact ENEMY");
            enemy.SetDestroyed();   //Destruye al impactado
            SetDestroyed();         //La bala se autodestruye
        }

        if (player != null)
        {
            //Debug.Log("Contact PLAYER");
            currentShield = player.GetComponent<Shield>();
            bool haveShield = currentShield.getShield();
            //Debug.Log($"Escudo -> {haveShield}");
            if (currentShield != null && haveShield)   //Si tiene escudo
            {
                currentShield.deactivateShield();   //Lo desactivo           
                //Debug.Log("Escudo Rotooo!!");
            }
            else   
                player.SetDestroyed();  //Destruye al impactado

            SetDestroyed();         //La bala se autodestruye
        }

        if (bullet != null)
        {
            //Debug.Log("Contact BULLET");
            bullet.SetDestroyed();  //Destruye al impactado
            SetDestroyed();         //La bala se autodestruye
        }
    }

    /*//Antiguo collider más grande para destruir otras balas más facilmente
    private void OnTriggerEnter(Collider other)
    private void OnTriggerStay(Collider other)
    {
        bullet = other.GetComponentInParent<Bullet>();
        if (bullet != null)
        {
            //Debug.Log("Contact BULLET");
            bullet.SetDestroyed();  //Destruye al impactado
            SetDestroyed();         //La bala se autodestruye
        }
    }*/


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
        /*//Evento Sonido Destroy
        if (onBulletSFXDestroy != null)
            onBulletSFXDestroy();*/

        //invulnerabilityTime = time;
        Destroy(gameObject);         //La bala se autodestruye
    }
}

//LA BALA EMPUJA AL TANQUE CUANDO SE ROMPE EL ESCUDO    ¿arreglarlo?