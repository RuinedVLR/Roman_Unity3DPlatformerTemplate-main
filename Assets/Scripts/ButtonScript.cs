using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] GameObject buttonText;
    Vector3 spawnPosition = new Vector3(11, 8, 9);
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            SpawnCube();
            buttonText.SetActive(true);
            audioSource.Play();
        }
        
        if(other.CompareTag("Cube"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            CloseDoor();
        }
    }

    void SpawnCube()
    {
        Instantiate(cube, spawnPosition, Quaternion.identity);
    }

    void OpenDoor()
    {
        GameObject.Find("Door").GetComponent<Animator>().Play("DoorOpen", 0, 0.0f);
    }

    void CloseDoor()
    {
        GameObject.Find("Door").GetComponent<Animator>().Play("DoorClose", 0, 0.0f);
    }
}
