using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public float designAspect = 16f / 9f;  

    private Camera cam;
    private float baseSize;
    private int lastW, lastH;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        baseSize = cam.orthographicSize;  
    }

    private void Update()
    {
        if (Screen.width == lastW && Screen.height == lastH) return;
        lastW = Screen.width;
        lastH = Screen.height;

        float aspect = (float)Screen.width / Screen.height;
        cam.orthographicSize = baseSize * Mathf.Max(1f, designAspect / aspect);
    }
}