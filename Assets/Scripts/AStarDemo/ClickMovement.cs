using UnityEngine;
using System.Collections;

public class ClickMovement : MonoBehaviour 
{
    public Transform targetTransform;
    [SerializeField] float movementSpeed, rotSpeed;

    private ArrayList path;
    private int currentWaypointIndex = 0;

	// Use this for initialization
	void Start () 
    {
        path = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (path.Count == 0 || currentWaypointIndex >= path.Count)
        {
            // Calculate path if the target is far enough
            if (Vector3.Distance(transform.position, targetTransform.position) > 0.5f)
            {
                Node startNode = GridManager.instance.GetNodeFromPosition(transform.position);
                Node goalNode = GridManager.instance.GetNodeFromPosition(targetTransform.position);

                path = AStar.FindPath(startNode, goalNode);
                currentWaypointIndex = 0;
            }
            return;
        }

        // Move to the next waypoint
        Node currentNode = (Node)path[currentWaypointIndex];
        Vector3 targetPosition = currentNode.position;

        // Rotate towards the waypoint
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);

        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // Check if the waypoint is reached
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;
        }
	}
}
