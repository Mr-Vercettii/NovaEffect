using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private Gun[] guns;

    public float acceleration = 250f;
    public float deceleration = 500f;
    public float maxSpeed = 20f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetVelocity = Vector3.zero;

    private float screenLeft, screenRight, screenBottom, screenTop;

    void Start()
    {
        animator = transform.Find("Engine").GetComponent<Animator>();

        // Calcular los límites de la pantalla
        float halfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        float halfHeight = Camera.main.orthographicSize;
        screenLeft = -halfWidth;
        screenRight = halfWidth;
        screenBottom = -halfHeight;
        screenTop = halfHeight;

        guns = transform.GetComponentsInChildren<Gun>();
    }

    void Update()
    {
        // Obtener las entradas de movimiento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 1. Dirección de movimiento normalizada
        targetVelocity = new Vector3(horizontal, vertical, 0f).normalized * maxSpeed;

        // 2. Animación del motor
        animator.SetBool("active_engine", (horizontal != 0 || vertical != 0));

        // 3. Disparo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Gun gun in guns)
                gun.Shoot();
        }

        // 4. Movimiento suave
        velocity = Vector3.Lerp(velocity, targetVelocity, acceleration * Time.deltaTime);
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (horizontal == 0f && vertical == 0f)
            velocity *= Mathf.Pow(1f - deceleration * Time.deltaTime, 2);

        // 5. Mantener en pantalla
        Vector3 newPos = transform.position + velocity * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, screenLeft, screenRight);
        newPos.y = Mathf.Clamp(newPos.y, screenBottom, screenTop);
        transform.position = newPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con un Destructible (enemigo), pierde una vida
        if (collision.GetComponent<Destructible>() != null)
        {
            // Destruye al enemigo
            Destroy(collision.gameObject);

            // Resta vida
            if (GameManager.Instance != null)
                GameManager.Instance.LoseLife();

            // ¡NO destruir aquí al jugador!
        }
    }
}



