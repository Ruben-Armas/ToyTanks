using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public Rigidbody rigidbody;

    /*Si lo pongo en el FixedUpdate estaría siempre aplicando esa velocidad
     * por lo que no caería por la gravedad
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
        // Comprueba si la colisión es con una pared
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Rebote");
            // Calcular la dirección de rebote
            Vector3 reflection = Vector3.Reflect(rigidbody.velocity, collision.contacts[0].normal);

            // Aplicar una fuerza en la dirección de rebote
            //rigidbody.AddForce(reflection * bounciness, ForceMode.Impulse);
            rigidbody.velocity = reflection * speed;
        }
        Debug.Log("NO  Rebote");
    }
}