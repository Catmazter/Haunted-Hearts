using UnityEngine;

public class DetectSafeZone : MonoBehaviour
{
    public bool InSafeZone { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("safeZone"))
        {
            InSafeZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("safeZone"))
        {
            InSafeZone = false;
        }
    }
}
