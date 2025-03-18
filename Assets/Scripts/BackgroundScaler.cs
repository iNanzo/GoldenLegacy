using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (spriteRenderer != null && mainCamera != null)
        {
            ScaleToFitScreen();
        }
    }
    void ScaleToFitScreen()
    {
        // Get the world height and width based on the camera
        float worldScreenHeight = mainCamera.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight * mainCamera.aspect;

        // Get the sprite's bounds
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Calculate the scale needed to fit the screen
        float scaleX = worldScreenWidth / spriteWidth;
        float scaleY = worldScreenHeight / spriteHeight;

        // Apply scale to fit the screen
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
