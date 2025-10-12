using UnityEngine;

public class MapRevealPainter : MonoBehaviour
{
    [Header("References")]
    public Camera mapCamera;        // stationary top-down camera rendering _MapTex
    public RenderTexture revealRT;  // the dynamic reveal texture
    public Material brushMaterial;
    public Transform player;        // the player object
    public float brushSize = 0.08f; // in normalized UV (0-1)

    private Vector3 mapBottomLeft;
    private Vector3 mapTopRight;

    void Start()
    {
        ClearRenderTexture();
        CalculateMapBounds();
    }

    void Update()
    {
        PaintAtPlayer();
    }

    void ClearRenderTexture()
    {
        var old = RenderTexture.active;
        RenderTexture.active = revealRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = old;
    }

    void CalculateMapBounds()
    {
        // Get the bottom-left and top-right points in world space based on orthographic camera
        float camHeight = mapCamera.orthographicSize;
        float camWidth = camHeight * mapCamera.aspect;

        Vector3 camPos = mapCamera.transform.position;
        mapBottomLeft = new Vector3(camPos.x - camWidth, 0, camPos.z - camHeight);
        mapTopRight = new Vector3(camPos.x + camWidth, 0, camPos.z + camHeight);
    }

    void PaintAtPlayer()
    {
        // Convert player world position to normalized UV coordinates (0-1)
        float u = Mathf.InverseLerp(mapBottomLeft.x, mapTopRight.x, player.position.x);
        float v = Mathf.InverseLerp(mapBottomLeft.z, mapTopRight.z, player.position.z);

        // Outside map bounds? skip painting
        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        // Send brush data to shader
        brushMaterial.SetVector("_BrushUV", new Vector4(u, v, brushSize, 0));

        // Blit the brush onto the render texture
        RenderTexture temp = RenderTexture.GetTemporary(revealRT.width, revealRT.height, 0, revealRT.format);
        Graphics.Blit(revealRT, temp);
        Graphics.Blit(temp, revealRT, brushMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }
}
