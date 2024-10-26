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
            tankObj.GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LowHealth);
            timerStart = false;
        }

        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 250.0f) // Slightly more than attack range
        {   
            tankObj.GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
            timerStart = false;
        }

        if (camoTimer <= 0)     // When timer ends, it tends to go right back into attack as player is right there and could double camo
        {
            tankObj.GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            tankObj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            Debug.Log("Leaving Camo State and entering Patrol State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
            timerStart = false;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (!timerStart)
        {
            camoTimer = Random.Range(5.0f, 10.0f);
            Debug.Log("CurrSpeed: " + curSpeed);
            tankObj.GetComponent<MeshRenderer>().enabled = false;
            tankObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            tankObj.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            timerStart = true;
        }
        Debug.Log(tankObj.GetComponent<MeshRenderer>().enabled); 
        camoTimer -= Time.deltaTime;
    }
}
