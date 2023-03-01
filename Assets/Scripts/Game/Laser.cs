using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask collisionLayer;

    void Update()
    {
        // Obtener los puntos del Line Renderer
        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);

        Debug.DrawLine(startPos, endPos*10, Color.green);
        // Detectar colisiones entre la l�nea y los objetos en la capa de colisi�n
        if (Physics.Linecast(startPos, endPos, out RaycastHit hitInfo, collisionLayer))
        {
            // Se ha detectado una colisi�n, hacer algo aqu�
            Debug.Log($"Colisi�n detectada con {hitInfo.collider.name}");
        }
    }
}
