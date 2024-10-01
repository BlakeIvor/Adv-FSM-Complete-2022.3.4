using UnityEngine;
using System.Collections;

public class PatrolState : FSMState
{

    private float timer;
    private bool camp;

    public PatrolState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Patrolling;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
        timer = -1;
        int prev = (int)Random.Range(1.0f, 3.0f);
        if (prev == 1)
        {
            camp = true;
        }
        else 
        {
            camp = false;
        }

    }

    public override void Reason(Transform player, Transform npc)
    {

        //1. Check the distance with player tank
        if (Vector3.Distance(npc.position, player.position) <= 300.0f)
        {
            //2. Since the distance is near, transition to chase state
            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        //1. Find another random patrol point if the current point is reached
        if (Vector3.Distance(npc.position, destPos) <= 100.0f)
        {
            Debug.Log("Reached to the destination point, calculating the next point");
            FindNextPoint();
        }

        //2. Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

    }
}