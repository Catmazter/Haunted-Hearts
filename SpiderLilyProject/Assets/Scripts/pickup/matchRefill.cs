using UnityEngine;

public class matchRefill : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // gameManager.instance.audioManager.Play("MatchPickup");
           // gameManager.instance.SendMessage("DisplayMatchPickupText");
           // gameManager.instance.playerScript.AddMatches(5);
        }
    }
}
