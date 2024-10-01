using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCampState : FSMState
{

    private bool timerStart = false;
    private float boredTimer;

    public NinjaCampState(Transform npc)
    {
        stateID = FSMStateID.Camp;
        curSpeed = 0.0f;
        boredTimer = 0;
    }

    public override void Reason(Transform player, Transform npc)
    {

        if (npc.GetComponent<NPCTankController>().health <= 30)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.NoHealth);
            timerStart = false;
        }

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
            timerStart = false;
        }
        else if (dist <= 200.0f)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.ReachPlayer);
            timerStart = false;
        }
        else if (boredTimer <= 0)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
            timerStart = false;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (!timerStart)
        {
            boredTimer = Random.Range(5.0f, 20.0f);
            timerStart = true;
        }

        boredTimer -= Time.deltaTime;
    }
}
