using UnityEngine;
using Unity.Cinemachine;

public class MyCameraCollision : CinemachineExtension
{
    public LayerMask obstacleLayers; // Layers that represent obstacles
    public float cameraRadius = 0.5f; // Radius of the camera for collision detection
    public float damping = 0.5f; // Damping factor for smooth camera movement
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 targetPos = state.ReferenceLookAt; // The position the camera is looking at
            Vector3 desiredPos = state.RawPosition; // The desired position of the camera
            Vector3 direction = desiredPos - targetPos; // Direction from the target to the camera
            float maxDistance = Vector3.Distance(desiredPos, targetPos); // Maximum distance from the target to the camera

            // Check for obstacles between the target and the desired camera position
            RaycastHit hit;
            if (Physics.SphereCast(targetPos, cameraRadius, direction.normalized, out hit, maxDistance, obstacleLayers))
            {
                Vector3 correctedPos = targetPos + direction.normalized * hit.distance; // Adjust the camera position to avoid the obstacle
                state.RawPosition = Vector3.Lerp(state.RawPosition, correctedPos, damping * deltaTime); // Smoothly move the camera to the corrected position
            }

            // Implement camera avoidance logic here
            // For example, you can cast a ray from the camera to the target and adjust the camera position if it hits an obstacle
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
