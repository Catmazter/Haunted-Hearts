using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private lilyScriptableObject pickupData;

    private void Start()
    {
        if(gameManager.instance != null)
        {
            gameManager.instance.updateGameGoal(1);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; 
        
        
        if (pickupData != null)
        {
            pickupData.OnPickup(other.gameObject);
            // play animation with coroutine and move destroy logic inside new couroutine
            Destroy(gameObject);
        }
    }
}