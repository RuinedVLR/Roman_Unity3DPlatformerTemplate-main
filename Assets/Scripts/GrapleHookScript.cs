using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapleHookScript : MonoBehaviour
{
    [Header("Grapple Hook Settings")]
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

            if (isHoldingGrapple)
            {
                targetPosition = nearest.transform.position;
                DrawRope();
            }
        }
    }

    // Gets all the colliders in whatIsGrapplable layer and with tag "GrapleTarget" that are inside the grappleRange sphere collider
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

        Collider[] hits = Physics.OverlapSphere(worldCenter, worldRadius, whatIsGrappleable.value);
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

        // Try read as float (1 when pressed, 0 when released) or as bool
        try
        {
            pressed = value.Get<float>() > 0.5f;
        }
        catch
        {
            try
            {
                pressed = value.Get<bool>();
            }
            catch
            {
                pressed = false;
            }
        }

        if (pressed)
        {
            Debug.Log("Grapple Hook Activated (InputValue)");
            isHoldingGrapple = true;
        }
        else
        {
            Debug.Log("Grapple Hook Deactivated (InputValue)");
            isHoldingGrapple = false;
            var rope = GameObject.FindWithTag("GrapleRope");
            if (rope != null) Destroy(rope);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (isHoldingGrapple && other.gameObject.layer == LayerMask.NameToLayer("Grapleable"))
        {
            targetPosition = other.transform.position;
            DrawRope();
        }
    }

    public void DrawRope()
    {
        if (GameObject.FindWithTag("GrapleRope") == null)
        {
            GameObject rope = new GameObject("Graple Rope");
            rope.tag = "GrapleRope";
            LineRenderer lineRenderer = rope.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, targetPosition);
        }
        else
        {
            GameObject rope = GameObject.FindWithTag("GrapleRope");
            LineRenderer lineRenderer = rope.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, targetPosition);
        }
    }
}
