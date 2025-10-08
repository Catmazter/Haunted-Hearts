using UnityEngine;

[CreateAssetMenu(menuName = "Lily")]
public class lilyScriptableObject : ScriptableObject , iPickup
{
    [Header("Pickup Settings")]
    public string pickupName = "New Pickup";
    public AudioClip pickupSound;

 

    public void OnPickup(GameObject pickup)
    {
        Debug.Log($"{pickup.name} picked up {pickupName}!");

        // Play pickup sound if assigned
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, pickup.transform.position);

        // Decrement GameManager value
        if (gameManager.instance != null)
        {
            gameManager.instance.updateGameGoal(-1);
        }
        else
        {
            Debug.LogWarning("GameManager instance not found!");
        }
    }
}