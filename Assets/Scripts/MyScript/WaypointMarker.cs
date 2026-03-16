using UnityEngine;


public class WaypointMarker : MonoBehaviour
{
    [SerializeField]
    public float waitTime = 1f; // 在此路停留的時間

    public Vector3 GetPosition() => transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);

        WaypointMarker[] allWaypoints = GetComponentsInParent<WaypointMarker>();
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            if (allWaypoints[i] != this)
            {
                WaypointMarker nextWaypoint = GetNextWaypoint();
                if (nextWaypoint != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, nextWaypoint.transform.position);
                }
            }
        }
    }

    public WaypointMarker GetNextWaypoint()
    {
        WaypointMarker[] waypoints = GetComponentsInParent<WaypointMarker>();

        if (waypoints.Length == 0)
            return null;

        int currentIndex = -1;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == this)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex >= 0)
        {
            int nextIndex = (currentIndex + 1) % waypoints.Length;
            return waypoints[nextIndex];
        }

        return null;
    }
}