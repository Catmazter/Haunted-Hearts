using UnityEngine;

public class MatchCollision : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.detectCollisions = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.detectCollisions = true;
        }
    }
}
