using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSin : MonoBehaviour
{
    float sinCenterY;
    public float amplitude = 2f; // Amplitud del movimiento sinusoidal
    public float frequency = 2f; // Frecuencia del movimiento sinusoidal
    public float phaseOffset = 0f; // Desfase de fase para sincronización
    public bool inverted = false;

    void Start()
    {
        sinCenterY = transform.position.y;
    }

    void FixedUpdate()
    {
        Vector2 pos = transform.position;
        
        // Calcular el valor de la onda sinusoidal con un desfase
        float sin = Mathf.Sin((pos.x * frequency) + phaseOffset) * amplitude;
        
        if (inverted)
        {
            sin *= -1;
        }

        pos.y = sinCenterY + sin;
        transform.position = pos;
    }
}
