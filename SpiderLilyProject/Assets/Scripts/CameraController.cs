using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [Range(100, 1000)][SerializeField] private int sensX = 653;
    [Range(100, 1000)][SerializeField] private int sensY = 653;
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

        sensX = PlayerPrefs.GetInt("SensitivityX", sensX);
        sensY = PlayerPrefs.GetInt("SensitivityY", sensY);
        FOV = PlayerPrefs.GetFloat("FOV", FOV);

    }


    // Update is called once per frame
    void Update()
    {
        changeFOV();

        //get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;
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

    public void SetSensitivityX(int value) => sensX = value;
    public void SetSensitivityY(int value) => sensY = value;
    public int GetSensitivityX() => sensX;
    public int GetSensitivityY() => sensY;
}

