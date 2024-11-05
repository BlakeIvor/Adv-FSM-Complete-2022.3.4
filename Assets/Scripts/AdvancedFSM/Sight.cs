using UnityEngine;
using System.Collections;

public class Sight : Sense
{
    public int FieldOfView = 45;
    public int ViewDistance = 100;

    private Transform playerTrans;
    private Vector3 rayDirection;

    protected override void Initialise() 
    {
        //set the value for the player transform -- playerTrans is the var name
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

	// Update is called once per frame
    protected override void UpdateSense() 
    {
        //update time passing in var elapsedTime

        //if enough time has passed [var in Sense.cs], poll for Aspect
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= detectionRate)
            Detect();
    }

    //Poll to detect perspective field of view for the AI Character
    void Detect()
    {
		//var to hold our RaycastHit
		RaycastHit hit;
		rayDirection = playerTrans.position - transform.position; //rayDirection set toward player position

		//if the direction angle toward our player is within our FieldOfView from us, then we care
		if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfView)
		{
            // apparently in FoV, now we test if player is within the DISTANCE of sight abilityusing raycast

            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                //if here, something is within sight, so see if what you hit with your raycast is of the ASPECT you are trying to detect
                if (hit.collider != null)
                {
                    //Check the aspect
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        //now console out -- Enemy Detected!
                        Debug.Log("Player Detected");
                    }
                }
            }

        } // end if angle between our AI-forward and player is within field of view
    } //end detectaspect

    /// <summary>
    /// Show Debug Grids and obstacles inside the editor
    /// </summary>
    void OnDrawGizmos()
    {
        //if (!debugMode || playerTrans == null)
		if (playerTrans == null)
            return;

        Debug.DrawLine(transform.position, playerTrans.position, Color.red);

        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        //Approximate perspective visualization
        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += FieldOfView * 0.5f;

        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= FieldOfView * 0.5f;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }
}
