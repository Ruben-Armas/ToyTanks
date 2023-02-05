using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public Rigidbody rigidbody;
    public int numRebotes;
    private int contRebotes;

    private EnemyController enemy;
    private Player player;

    /*Si lo pongo en el FixedUpdate estaría siempre aplicando esa velocidad
     * por lo que no caería por la gravedad
     * Pero si lo hago en el Start() -> tendremos balas con caida
    */
    public void Start()
    {//No hace falta el deltaTime porque solo se usa cuando "se suma"
        rigidbody.velocity = transform.forward * speed;
        contRebotes = 0;
    }

    private void Update()
    {
        Debug.DrawLine(rigidbody.velocity, transform.forward, Color.black);
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rigidbody.velocity, out hit, 1)) // Comprueba si choca
        {
            if (hit.collider.GetComponentInParent<Wall>() != null)
            {
                // Calcula la dirección de rebote
                Vector3 reflectionDirection = Vector3.Reflect(rigidbody.velocity, hit.normal);
                transform.forward = reflectionDirection;            // Gira a la nueva dirección
                rigidbody.velocity = transform.forward * speed;     // Mueve la bala
                Debug.Log($"reflection -> {hit.normal}");
            }
        }
        */
    }
    /*Bala con velocidad constante (siempre aplicando esa velocidad)
    private void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * speed;
    }*/

    //Hacer que cuente los choques y que se destruya al segundo choque
    //Hacer que rebote cuando choca con la pared
    private void OnCollisionEnter(Collision collision)
    {        
        // Comprueba si la colisión es con una pared
        if (collision.collider.GetComponentInParent<Wall>() != null)
        //if (collision.collider.GetComponent<Wall>() != null)
        {
            //Debug.Log("Rebote");
            if (checkIfSelfDestroy() == false)
            {
                // Calcula la normal de la superficie de la pared con la que chocando
                Vector3 contact = collision.contacts[0].normal;
                //ContactPoint contact = collision.GetContact(0).normal;
                Debug.Log($"contact -> {contact}");

                // Calcula la dirección de rebote
                //Vector3 reflectedDirection = Vector3.Reflect(rigidbody.velocity, contact);
                Vector3 reflectedDirection = Vector3.Reflect(rigidbody.velocity, Vector3.up);
                Debug.Log($"reflectedDirection -> {reflectedDirection}");

                // Gira a la nueva dirección
                transform.forward = reflectedDirection.normalized;

                //Reinicio la inercia
                //rigidbody.velocity = Vector3.zero;

                // Mueve la bala
                //rigidbody.velocity = reflectedDirection;
                rigidbody.velocity = transform.forward * speed;
            }
        }
        //Debug.Log("NO Rebota");

        enemy = collision.collider.GetComponentInParent<EnemyController>();
        player = collision.collider.GetComponentInParent<Player>();
        if (enemy != null)
        {
            Debug.Log("CONTACTO");
            enemy.SetDestroyed();
            Destroy(gameObject);    //La bala se autodestruye
        }
        if (player != null)
        {
            Debug.Log("CONTACTO");
            player.SetDestroyed();
            Destroy(gameObject);    //La bala se autodestruye
        }
        //checkIfcollisionDestroy(enemy);
    }
    /*
    private void checkIfcollisionDestroy(Object collisioned)
    {
        if (collisioned != null)
        {
            Debug.Log("CONTACTO");
            collisioned.SetDestroyed();
            Destroy(gameObject);
        }
    }
    */
    private bool checkIfSelfDestroy()
    {
        contRebotes++;
        if (contRebotes >= numRebotes)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }


    /*
    //REBOTE POR CODIGO
    Vector3 towardsMe = transform.position - collision.transform.position;
    //Si lo normalizamos, podremos añadir siempre el mismo impulso sin importar la distancia o tamaño de la bola
    towardsMe = towardsMe.normalized;
    Debug.DrawLine(collision.transform.position, collision.transform.position + towardsMe, Color.red, 2);
    rigidbody.AddForce(towardsMe, ForceMode.Impulse);
    */
}