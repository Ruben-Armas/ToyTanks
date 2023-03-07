using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [Range(0.1f, 20f)]
    public float hitDistance = 10f;

    public Color freeColor = Color.green;
    public Color collisionColor = Color.red;

    private Material _material;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        _material = lineRenderer.material;
    }

    void Update()
    {
        RaycastHit hitWall;
        //Debug.DrawLine(transform.position, transform.position + transform.forward * hitDistance, Color.white);
        if (Physics.Raycast(transform.position, transform.forward, out hitWall, hitDistance)) // Comprueba si choca
        {
            // Comprueba si la colisión es con una pared
            if (hitWall.collider.GetComponentInParent<Wall>() != null)
            {
                _material.color = collisionColor;
                //lineRenderer.startColor = freeColor;
                //lineRenderer.endColor = freeColor;
            }
        }
        else
            _material.color = freeColor;
    }
}
