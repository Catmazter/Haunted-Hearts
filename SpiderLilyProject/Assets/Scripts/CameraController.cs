using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [UnityEngine.Range(100, 1000)][SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    [SerializeField] public float FOV;
    float FOVOrig;

    float rotX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<Camera>();
        FOVOrig = FOV;
    }

    // Update is called once per frame
    void Update()
    {
        changeFOV();
        
        //get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;
        //use invertY to give option of look up/down
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        //clamp the camera on the X axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate the camera to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the player left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
    void changeFOV()
    {
        cam.fieldOfView = FOV;
    }
}
