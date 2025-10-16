using UnityEngine;

public class ScareTrigger : MonoBehaviour
{
    public Animator anim;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("scareTrigger");
        }
    }
}