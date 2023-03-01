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
        // Detectar colisiones entre la línea y los objetos en la capa de colisión
        if (Physics.Linecast(startPos, endPos, out RaycastHit hitInfo, collisionLayer))
        {
            // Se ha detectado una colisión, hacer algo aquí
            Debug.Log($"Colisión detectada con {hitInfo.collider.name}");
        }
    }
}
