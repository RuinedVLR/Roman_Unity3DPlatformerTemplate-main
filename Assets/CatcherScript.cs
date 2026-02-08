using UnityEngine;

public class CatcherScript : MonoBehaviour
{
    [SerializeField] GameObject startPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = startPoint.transform.position;
        }

        if (other.CompareTag("Cube"))
        {
            Destroy(other.gameObject);
        }
    }
}
