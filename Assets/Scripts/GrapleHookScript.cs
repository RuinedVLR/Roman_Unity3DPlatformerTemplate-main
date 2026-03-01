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
    [SerializeField] float grappleForce;
    LineRenderer lr;

    [Header("Target Settings")]
    
    [SerializeField] LayerMask whatIsGrappleable;
    
    Vector3 targetPosition;

    bool isHoldingInput = false;
    bool isGrappling = false;

    GameObject rope;
    LineRenderer ropeRenderer;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // check for grapplable targets inside the grapple range
        var grapplables = GetGrapplableTargetsInsideRange();
        if (grapplables.Length > 0)
        {
            // choose the nearest one
            Collider nearest = grapplables[0];
            float nearestDist = Vector3.Distance(grappleOrigin.position, nearest.transform.position);
            for (int i = 1; i < grapplables.Length; i++)
            {
                float d = Vector3.Distance(grappleOrigin.position, grapplables[i].transform.position);
                if (d < nearestDist)
                {
                    nearest = grapplables[i];
                    nearestDist = d;
                }
            }

            targetPosition = nearest.transform.position;

            if (isHoldingInput)
            {
                isGrappling = true; 
                playerRb.AddForce((targetPosition - player.position).normalized * grappleForce, ForceMode.Acceleration);
            }
            else
            {
                isGrappling = false;
            }
        }
        
        if(grapplables.Length == 0)
        {
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
            LineRenderer lineRenderer = rope.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            if(lineRenderer.material == null)
            {
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, targetPosition);
        }
        else
        {
            rope = GameObject.FindWithTag("GrapleRope");
            ropeRenderer = rope.GetComponent<LineRenderer>();
            ropeRenderer.startColor = Color.black;
            ropeRenderer.endColor = Color.black;
            if (ropeRenderer.material == null)
            {
                ropeRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
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
