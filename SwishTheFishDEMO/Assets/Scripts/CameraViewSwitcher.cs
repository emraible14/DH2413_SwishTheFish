using UnityEngine;

public class CameraViewSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Color fallbackColor = Color.blue;

    private Color originalColor;
    private bool fallbackActive = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        originalColor = mainCamera.backgroundColor;
    }

    void Update()
    {
        // Exemple : bascule manuelle avec espace
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleView();
        }
    }

    public void ToggleView()
    {
        fallbackActive = !fallbackActive;

        if (fallbackActive)
        {
            // Cache la scène
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = fallbackColor;
            mainCamera.cullingMask = 0; // Ne rend aucun layer
        }
        else
        {
            // Rétablit la vue normale
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            mainCamera.backgroundColor = originalColor;
            mainCamera.cullingMask = ~0; // Rend tous les layers
        }
    }
}
