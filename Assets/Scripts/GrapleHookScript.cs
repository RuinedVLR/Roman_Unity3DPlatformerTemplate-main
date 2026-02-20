using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapleHookScript : MonoBehaviour
{
    [Header("Graple Hook Settings")]
    [SerializeField] Transform player;
    [SerializeField] Transform grappleOrigin;
    [SerializeField] SphereCollider grappleRange;
    LineRenderer lr;

    [Header("Target Settings")]
    [SerializeField] GameObject target;
    [SerializeField] LayerMask whatIsGrappleable;
    
    Vector3 targetPosition;

    bool isHoldingGrapple = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        targetPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGrappleHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingGrapple = true;
        }
        else if (context.canceled)
        {
            isHoldingGrapple = false;
            Destroy(GameObject.FindWithTag("GrapleRope"));
        }


    }
}
