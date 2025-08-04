using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Canvas canvas;
    private Text titleText;
    private Button playButton;
    private Button quitButton;
    

    void Start()
    {
        // Crear y configurar Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        // Crear el texto del título
        GameObject titleObject = new GameObject("TitleText");
        titleObject.transform.SetParent(canvas.transform);
        titleText = titleObject.AddComponent<Text>();
        titleText.text = "Nova Effect";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Cambia aquí si tienes otra fuente
        titleText.fontSize = 50;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.rectTransform.anchoredPosition = new Vector2(0, 100);
        titleText.rectTransform.sizeDelta = new Vector2(600, 100);

        // Crear el texto del autor
        GameObject authorObject = new GameObject("AuthorText");
        authorObject.transform.SetParent(canvas.transform);
        Text authorText = authorObject.AddComponent<Text>();
        authorText.text = "Vercetti Productions";
        authorText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Cambia aquí si tienes otra fuente
        authorText.fontSize = 30;
        authorText.alignment = TextAnchor.MiddleCenter;
        authorText.color = Color.white;
        authorText.rectTransform.anchoredPosition = new Vector2(0, 50); // Posición debajo del título
        authorText.rectTransform.sizeDelta = new Vector2(600, 50);

        // Crear el botón de Play
        playButton = CreateButton("PlayButton", "Jugar", new Vector2(0, -20), PlayGame);

        // Crear el botón de Quit
        quitButton = CreateButton("QuitButton", "Salir", new Vector2(0, -100), QuitGame);
    }


    // Método para crear botones
    private Button CreateButton(string name, string text, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(canvas.transform);

        // Asegurarse de que el objeto tenga los componentes necesarios
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 60); // Tamaño del botón
        rectTransform.anchoredPosition = position; // Posición del botón
        rectTransform.localScale = Vector3.one;

        buttonObject.AddComponent<CanvasRenderer>();

        // Agregar componente de imagen y color al botón
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = Color.white;

        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(action);

        // Crear un objeto separado para el texto del botón
        GameObject buttonTextObject = new GameObject("ButtonText");
        buttonTextObject.transform.SetParent(buttonObject.transform);

        Text buttonText = buttonTextObject.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Cambia aquí si tienes otra fuente
        buttonText.fontSize = 30;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;

        // Asegurarse de que el texto tenga un RectTransform y esté centrado en el botón
        RectTransform textRectTransform = buttonText.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = rectTransform.sizeDelta;
        textRectTransform.anchoredPosition = Vector2.zero;
        textRectTransform.localScale = Vector3.one;

        return button;
    }

    // Método para iniciar el juego https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html
    private void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    // Método para salir del juego
    private void QuitGame()
    {
        Debug.Log("Quit button clicked"); // Agrega este mensaje para verificar que se ejecute
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
