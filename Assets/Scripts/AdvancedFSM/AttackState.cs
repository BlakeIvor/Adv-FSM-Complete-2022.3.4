using UnityEngine;
using System.Collections;

public class AttackState : FSMState
{
    private bool decidedAttack = false;
    
    public AttackState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (!decidedAttack)
        {
            float randNum = Random.Range(0.0f, 1.0f);
            if (randNum <= (1.0f - npc.parent.childCount/5.0f))
            {
                npc.GetComponent<NPCTankController>().SetTransition(Transition.CamoAttack);
                decidedAttack = false;
            }
            else
            {
                decidedAttack = true;
            }
        }


        //Check the distance with the player tank
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 200.0f && dist < 300.0f)
        {
            //Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);
            decidedAttack = false;
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            Debug.Log("Switch to Patrol State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
            decidedAttack = false;
        }  
    }

    public override void Act(Transform player, Transform npc)
    {
        if (decidedAttack)
        {
            //Set the target position as the player position
            destPos = player.position;

            //Always Turn the turret towards the player
            Transform turret = npc.GetComponent<NPCTankController>().turret;
            Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
            turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

            //Shoot bullet towards the player
            npc.GetComponent<NPCTankController>().ShootBullet();
        }
    }
}
