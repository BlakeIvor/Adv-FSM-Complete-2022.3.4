using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoredState : FSMState
{
    private float timer;
    private float boredTimer;

    public BoredState(Transform npc)
    {
        stateID = FSMStateID.Bored;
        curSpeed = 0.0f;
        timer = -1.0f;
        boredTimer = 0;
    }

    public override void Reason(Transform player, Transform npc)
    {
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
        }
        else if (boredTimer <= 0)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (timer == -1.0f)
        {
            boredTimer = Random.Range(5.0f, 20.0f);
        }
        Transform turret = npc.gameObject.transform.GetChild(0).transform;
        turret.Rotate(0, 45f * Time.deltaTime, 0);
        boredTimer -= Time.deltaTime;
    }
}