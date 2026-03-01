using UnityEngine;

public class RotateGrapplingGun : MonoBehaviour
{
    public GrapleHookScript grappling;

    Quaternion desiredRotation;
    float rotationSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        if(!grappling.GetIsGrappling())
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappling.GetTargetPosition() - transform.position);
        }

        transform.rotation = Quaternion.Lerp( a: transform.rotation, b: desiredRotation, t: Time.deltaTime * rotationSpeed);
    }
}
