using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public Rigidbody rigidbody;

    /*Si lo pongo en el FixedUpdate estar�a siempre aplicando esa velocidad
     * por lo que no caer�a por la gravedad
     * Pero si lo hago en el Start() -> tendremos balas con caida
    */ 
    public void Start()
    {//No hace falta el deltaTime porque solo se usa cuando "se suma"
        rigidbody.velocity = transform.forward * speed;
    }

    //Hacer que cuente los choques y que se destruya al segundo choque
    //Mirar si se utilizaban los Tags u otra cosa
    //Hacer que rebote cuando choca con la pared
    private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si la colisi�n es con una pared
        //if (collision.collider.tag == "Wall") //Evitamos usar los Tags
        //if (collision.gameObject.CompareTag("Wall"))
        if (collision.collider.GetComponentInParent<Wall>() != null)
        //if (collision.collider.GetComponent<Wall>() != null)
        {
            Debug.Log("Rebote");
            // Calcular la direcci�n de rebote
            Vector3 reflection = Vector3.Reflect(rigidbody.velocity, collision.contacts[0].normal);

            // Aplicar una fuerza en la direcci�n de rebote
            //rigidbody.AddForce(reflection * bounciness, ForceMode.Impulse);
            rigidbody.velocity = reflection * speed;
        }
        //Debug.Log("NO  Rebote");
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider.tag == "Player") //Evitamos usar los Tags

        if (collision.collider.IsPlayer())   //M�todo de extensi�n
        //if (collision.collider.GetComponent<PeggleBall>() != null)    //Lo mismo que el anterior
        {
            if (!isActive)
                Activate();
        }
    }*/
}