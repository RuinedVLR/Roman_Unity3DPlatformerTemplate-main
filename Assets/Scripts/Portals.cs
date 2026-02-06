using UnityEngine;

public class Portals : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the portal");
            other.transform.position = targetPosition.position;
        }
    }
}
