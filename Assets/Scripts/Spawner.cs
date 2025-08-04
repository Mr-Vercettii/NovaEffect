using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject eliteEnemyPrefab; // Prefab de los enemigos de elite
    public float spawnInterval = 5f; // Intervalo de tiempo entre cada oleada
    public int minEnemiesPerWave = 4; // Mínimo de enemigos por oleada
    public int maxEnemiesPerWave = 8; // Máximo de enemigos por oleada
    public float amplitude = 2f; // Amplitud común para el movimiento sinusoidal
    public float frequency = 1.5f; // Frecuencia común para el movimiento sinusoidal
    public float xSpawnRangeMin = 15f; // Rango mínimo de aparición en X
    public float xSpawnRangeMaxDefault = 19f; // Rango máximo base de aparición en X

    private void Start()
    {
        StartCoroutine(SpawnSynchronizedWaves());
    }

    private IEnumerator SpawnSynchronizedWaves()
    {
        while (true)
        {
            // Generar un número aleatorio de enemigos para esta oleada
            int enemiesPerWave = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);

            // Ajustar el rango máximo de aparición en X aleatoriamente entre 30 y 40
            float xSpawnRangeMax = Random.Range(30f, 40f);

            List<GameObject> waveEnemies = new List<GameObject>();

            for (int i = 0; i < enemiesPerWave; i++)
            {
                // Distribuir a los enemigos en X de forma equidistante entre el rango mínimo y el máximo aleatorio
                float spawnX = Mathf.Lerp(xSpawnRangeMin, xSpawnRangeMax, (float)i / (enemiesPerWave - 1));
                Vector3 spawnPosition = new Vector3(spawnX, 0f, 0f);

                GameObject newEnemy = Instantiate(eliteEnemyPrefab, spawnPosition, Quaternion.identity);
                waveEnemies.Add(newEnemy);

                //Se puede destruir
                Destructible DestructibleScript = newEnemy.AddComponent<Destructible>();

                // Configurar el movimiento sinusoidal sincronizado para toda la oleada
                MoveSin moveSinScript = newEnemy.AddComponent<MoveSin>();
                moveSinScript.amplitude = amplitude;
                moveSinScript.frequency = frequency;
                moveSinScript.phaseOffset = i * (Mathf.PI / enemiesPerWave); // Ajuste de desfase para movimiento sincronizado
            }

            // Esperar a que todos los enemigos de la oleada se hayan destruido
            while (waveEnemies.Count > 0)
            {
                for (int i = waveEnemies.Count - 1; i >= 0; i--)
                {
                    if (waveEnemies[i] == null)
                    {
                        waveEnemies.RemoveAt(i);
                    }
                }
                yield return null;
            }

            // Esperar antes de la siguiente oleada
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
