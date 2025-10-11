using UnityEngine;

public class MapRevealPainter : MonoBehaviour
{
    public Camera revealCam;
    public Material drawMaterial; // DrawCircleMaterial
    public RenderTexture revealRT;
    public float revealRadius = 0.05f;

    void Start()
    {
        // Initialize RevealRT to black
        RenderTexture.active = revealRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }

    void Update()
    {
        Vector3 uvPos = revealCam.WorldToViewportPoint(transform.position);
        if (uvPos.z > 0)
        {
            drawMaterial.SetVector("_Center", new Vector4(uvPos.x, uvPos.y, 0, 0));
            drawMaterial.SetFloat("_Radius", revealRadius);
            Graphics.Blit(null, revealRT, drawMaterial);
        }
    }
}
