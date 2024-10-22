using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamoState : FSMState
{
    private bool timerStart = false;
    private float camoTimer;
    private GameObject tankObj;

    public CamoState(Transform npc)
    {
        stateID = FSMStateID.Camo;
        curSpeed = 0.0f;
        camoTimer = 0.0f;
        tankObj = npc.GetComponent<NPCTankController>().gameObject;
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (npc.GetComponent<NPCTankController>().health <= 30)
        {

        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (!timerStart)
        {
            camoTimer = Random.Range(5.0f, 10.0f);
            timerStart = true;
        }
        tankObj.GetComponent<MeshRenderer>().enabled = false;
        camoTimer -= Time.deltaTime;
    }
}
