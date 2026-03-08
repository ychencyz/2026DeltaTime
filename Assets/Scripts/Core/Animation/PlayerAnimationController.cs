using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Transform headBone; // Drag the character's head bone Transform here in the Inspector
    public Transform targetToLookAt; // Drag the target object (e.g., the camera transform) here
    public Quaternion Offset = Quaternion.Euler(0, 115, 288);
    void LateUpdate()
    {
        //if (headBone != null && targetToLookAt != null)
        //{
        //    // Make the head bone look at the target position
        //    headBone.LookAt(targetToLookAt.position);

        //    // Optional: adjust the rotation if the character's model has an unusual forward axis
        //    headBone.rotation *= Offset; 
        //}
    }
}
