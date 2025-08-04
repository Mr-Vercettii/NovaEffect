using UnityEngine;
using System.Collections.Generic;

public class Background : MonoBehaviour
{
    [System.Serializable]
    public class StarLayer
    {
        public int starCount = 100;
        public float scrollSpeed = 1f;
        public float minSize = 0.02f;
        public float maxSize = 0.1f;
        public float twinkleSpeed = 2f;
        public float twinkleIntensity = 0.5f;
    }

    [System.Serializable]
    public class NebulaLayer
    {
        public int nebulaCount = 3;
        public float scrollSpeed = 0.3f;
        public float minSize = 5f;
        public float maxSize = 10f;
        public Color[] possibleColors;
    }

    [Header("Stars Configuration")]
    public StarLayer[] starLayers;

    [Header("Nebula Configuration")]
    public NebulaLayer nebulaLayer;

    [Header("Boundaries")]
    public float horizontalBoundary = 30f;
    public float verticalBoundary = 15f;

    private class ProceduralStar
    {
        public GameObject gameObject;
        public float twinklePhase;
        public float baseAlpha;
        public SpriteRenderer spriteRenderer;
    }

    private List<List<ProceduralStar>> layerStars = new List<List<ProceduralStar>>();
    private List<GameObject> nebulas = new List<GameObject>();

    void Start()
    {
        InitializeBackground();
    }

    void InitializeBackground()
    {
        // Generar capas de estrellas
        foreach (var layer in starLayers)
        {
            var stars = new List<ProceduralStar>();
            for (int i = 0; i < layer.starCount; i++)
            {
                CreateStar(layer, stars);
            }
            layerStars.Add(stars);
        }

        // Generar nebulosas
        for (int i = 0; i < nebulaLayer.nebulaCount; i++)
        {
            CreateNebula();
        }
    }

    void CreateStar(StarLayer layer, List<ProceduralStar> stars)
    {
        // Crear objeto para la estrella
        GameObject starObj = new GameObject("Star");
        starObj.transform.SetParent(transform);

        // Posición aleatoria
        starObj.transform.position = GetRandomPosition();

        // Añadir SpriteRenderer y configurar la estrella
        SpriteRenderer renderer = starObj.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateStarSprite();
        float size = Random.Range(layer.minSize, layer.maxSize);
        starObj.transform.localScale = new Vector3(size, size, 1);

        // Color base aleatorio con tinte azulado/blanquecino
        Color starColor = Color.Lerp(Color.white, new Color(0.8f, 0.9f, 1f), Random.value);
        float baseAlpha = Random.Range(0.5f, 1f);
        renderer.color = starColor * baseAlpha;

        // Crear objeto ProceduralStar y añadirlo a la lista
        ProceduralStar star = new ProceduralStar
        {
            gameObject = starObj,
            twinklePhase = Random.Range(0f, 2f * Mathf.PI),
            baseAlpha = baseAlpha,
            spriteRenderer = renderer
        };
        stars.Add(star);
    }

    void CreateNebula()
    {
        // Crear objeto para la nebulosa
        GameObject nebulaObj = new GameObject("Nebula");
        nebulaObj.transform.SetParent(transform);
        nebulaObj.transform.position = GetRandomPosition();

        // Añadir SpriteRenderer
        SpriteRenderer renderer = nebulaObj.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateNebulaSprite();
        
        // Tamaño aleatorio
        float size = Random.Range(nebulaLayer.minSize, nebulaLayer.maxSize);
        nebulaObj.transform.localScale = new Vector3(size, size, 1);

        // Color aleatorio de la paleta definida
        if (nebulaLayer.possibleColors != null && nebulaLayer.possibleColors.Length > 0)
        {
            // Seleccionar un color aleatorio de la paleta
            int randomIndex = Random.Range(0, nebulaLayer.possibleColors.Length);
            Color baseColor = nebulaLayer.possibleColors[randomIndex];

            // Aplicar el color con un alpha aleatorio
            renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, Random.Range(0.8f, 0.9f));
        }
        else
        {
            // Si no se ha definido una paleta de colores, usar un color predefinido
            renderer.color = new Color(0.5f, 0.7f, 1.0f, 0.8f);
        }

        nebulas.Add(nebulaObj);
    }

    Sprite CreateStarSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Max(0, 1 - (distance / (size / 4)));
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha * alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }

    Sprite CreateNebulaSprite()
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        
        // Generar ruido Perlin para la forma de la nebulosa
        float scale = Random.Range(4f, 8f);
        float offsetX = Random.Range(0f, 1000f);
        float offsetY = Random.Range(0f, 1000f);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float xCoord = (float)x / size * scale + offsetX;
                float yCoord = (float)y / size * scale + offsetY;
                
                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(size/2, size/2));
                float edgeFade = Mathf.Max(0, 1 - (distance / (size/2)));
                
                float alpha = noise * edgeFade;
                alpha = Mathf.Pow(alpha, 1.5f); // Hacer los bordes más suaves
                
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }

    void Update()
    {
        // Actualizar estrellas
        for (int layerIndex = 0; layerIndex < starLayers.Length; layerIndex++)
        {
            var layer = starLayers[layerIndex];
            foreach (var star in layerStars[layerIndex])
            {
                // Efecto de parpadeo
                star.twinklePhase += Time.deltaTime * layer.twinkleSpeed;
                float twinkle = 1 + (Mathf.Sin(star.twinklePhase) * layer.twinkleIntensity);
                star.spriteRenderer.color = new Color(
                    star.spriteRenderer.color.r,
                    star.spriteRenderer.color.g,
                    star.spriteRenderer.color.b,
                    star.baseAlpha * twinkle
                );

                // Movimiento parallax
                star.gameObject.transform.Translate(Vector3.left * layer.scrollSpeed * Time.deltaTime);

                // Reposicionar si sale de la pantalla
                if (star.gameObject.transform.position.x < -horizontalBoundary)
                {
                    Vector3 newPos = star.gameObject.transform.position;
                    newPos.x = horizontalBoundary;
                    newPos.y = Random.Range(-verticalBoundary, verticalBoundary);
                    star.gameObject.transform.position = newPos;
                }
            }
        }

        // Actualizar nebulosas
        foreach (var nebula in nebulas)
        {
            nebula.transform.Translate(Vector3.left * nebulaLayer.scrollSpeed * Time.deltaTime);

            if (nebula.transform.position.x < -horizontalBoundary * 1.5f)
            {
                Vector3 newPos = nebula.transform.position;
                newPos.x = horizontalBoundary * 1.5f;
                newPos.y = Random.Range(-verticalBoundary, verticalBoundary);
                nebula.transform.position = newPos;
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-horizontalBoundary, horizontalBoundary),
            Random.Range(-verticalBoundary, verticalBoundary),
            0
        );
    }
}