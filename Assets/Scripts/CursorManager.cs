using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    [Header("Cursor Textures")]
    public Texture2D crosshairTexture;

    [Tooltip("Hotspot (pivot) of the cursor image, e.g., center for crosshair")]
    public Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // persist across scenes
    }

    private void Start()
    {
        SetDefaultCursor(); // In case you're in menu initially
    }

    public void SetCrosshairCursor()
    {
        Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
