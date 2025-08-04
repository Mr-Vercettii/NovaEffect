using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int scoreValue = 10;
    bool canBeDestroyed = false;

    void Update()
    {
        if (transform.position.x < 10f)
            canBeDestroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeDestroyed) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(scoreValue);

            Destroy(bullet.gameObject);
            Destroy(gameObject);
        }
    }
}

