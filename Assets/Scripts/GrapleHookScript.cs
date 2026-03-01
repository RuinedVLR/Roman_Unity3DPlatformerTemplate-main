using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapleHookScript : MonoBehaviour
{
    [Header("Grapple Hook Settings")]
    [SerializeField] Transform player;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] Transform grappleOrigin;
    [SerializeField] SphereCollider grappleRange;
    [SerializeField] float grappleForce = 70;
    [SerializeField] Material ropeMaterial;
    
    LineRenderer lr;

    [Header("Target Settings")]
    [SerializeField] LayerMask whatIsGrappleable;

    [Header("Audio Clips")]
    [SerializeField] AudioClip grappleOn;
    [SerializeField] AudioClip grappleOff;

    AudioSource source;

    Vector3 targetPosition;

    bool isHoldingInput = false;
    bool isGrappling = false;

    bool hasPlayedGrappleOnSound = false;
    bool hasPlayedGrappleOffSound = true;

    GameObject rope;
    LineRenderer ropeRenderer;

    Collider lockedTarget = null;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingInput)
        {
            // If target locked
            if (lockedTarget != null)
            {
                // check if target in radius
                float dist = Vector3.Distance(grappleOrigin.position, lockedTarget.transform.position);
                float maxRadius = grappleRange.radius * grappleRange.transform.lossyScale.x;

                if (dist > maxRadius)
                {
                    // if target is out of range - reset
                    lockedTarget = null;
                    isGrappling = false;
                    return;
                }

                // if target in radius - pull
                targetPosition = lockedTarget.transform.position;

                if (!hasPlayedGrappleOnSound)
                {
                    source.PlayOneShot(grappleOn);
                    hasPlayedGrappleOnSound = true;
                }

                isGrappling = true;
                playerRb.AddForce((targetPosition - player.position).normalized * grappleForce, ForceMode.Acceleration);
            }
            else
            {
                // if not locked - find a new target
                var grapplables = GetGrapplableTargetsInsideRange();
                if (grapplables.Length > 0)
                {
                    lockedTarget = GetNearest(grapplables);
                    targetPosition = lockedTarget.transform.position;
                }
            }
        }
        else
        {
            // if button released - reset
            lockedTarget = null;
            isGrappling = false;
        }
    }

    void LateUpdate()
    {
        if (isGrappling)
        {
            Debug.Log("Drawing Grapple Rope");
            DrawRope();
        }
        else
        {
            var rope = GameObject.FindWithTag("GrapleRope");
            if (rope != null) Destroy(rope);
        }
    }

    private Collider GetNearest(Collider[] list)
    {
        Collider nearest = list[0];
        float nearestDist = Vector3.Distance(grappleOrigin.position, nearest.transform.position);

        for (int i = 1; i < list.Length; i++)
        {
            float d = Vector3.Distance(grappleOrigin.position, list[i].transform.position);
            if (d < nearestDist)
            {
                nearest = list[i];
                nearestDist = d;
            }
        }

        return nearest;
    }

    // gets all the colliders in whatIsGrapplable layer and with tag "GrapleTarget" that are inside the grappleRange sphere collider
    private Collider[] GetGrapplableTargetsInsideRange()
    {
        if (grappleRange == null) return new Collider[0];

        // world position of the center of sphere collider
        Vector3 worldCenter = grappleRange.transform.TransformPoint(grappleRange.center);
        // radius scaled by the global scale
        float worldRadius = grappleRange.radius * Mathf.Max(
            Mathf.Abs(grappleRange.transform.lossyScale.x),
            Mathf.Abs(grappleRange.transform.lossyScale.y),
            Mathf.Abs(grappleRange.transform.lossyScale.z)
        );

        Collider[] hits = Physics.OverlapSphere(worldCenter, worldRadius, whatIsGrappleable);
        if (hits == null || hits.Length == 0) return new Collider[0];

        // filter hits by tag "GrapleTarget"
        var list = new System.Collections.Generic.List<Collider>(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].CompareTag("GrapleTarget"))
                list.Add(hits[i]);
        }
        return list.ToArray();
    }

    public void OnGrapplehook(InputValue value)
    {
        if (value == null)
            return;

        hasPlayedGrappleOffSound = false;
        hasPlayedGrappleOnSound = false;

        bool pressed = false;

        pressed = value.isPressed;

        if (pressed)
        {
            Debug.Log("Grapple Hook Activated (InputValue)");
            isHoldingInput = true;
        }
        else if (!pressed)
        {
            Debug.Log("Grapple Hook Deactivated (InputValue)");
            isHoldingInput = false;
            var rope = GameObject.FindWithTag("GrapleRope");
            if (rope != null) Destroy(rope);
        }
    }

    public void DrawRope()
    {
        if (GameObject.FindWithTag("GrapleRope") == null)
        {
            rope = new GameObject("Graple Rope");
            rope.tag = "GrapleRope";
            LineRenderer ropeRenderer = rope.AddComponent<LineRenderer>();
            ropeRenderer.startColor = Color.black;
            ropeRenderer.endColor = Color.black;
            ropeRenderer.material = ropeMaterial;
            ropeRenderer.startWidth = 0.1f;
            ropeRenderer.endWidth = 0.1f;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, grappleOrigin.position);
            ropeRenderer.SetPosition(1, targetPosition);
        }
        else
        {
            rope = GameObject.FindWithTag("GrapleRope");
            ropeRenderer = rope.GetComponent<LineRenderer>();
            ropeRenderer.startColor = Color.black;
            ropeRenderer.endColor = Color.black;
            ropeRenderer.material = ropeMaterial;
            ropeRenderer.SetPosition(0, grappleOrigin.position);
            ropeRenderer.SetPosition(1, targetPosition);
        }
    }

    public bool GetIsGrappling()
    {
        return isGrappling;
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
}
