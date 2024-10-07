using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OffDutyState : FSMState
{
    public static List<Transform> offDutyTanks = new List<Transform>();

    private float timer;
    private bool timerStart = false;

    Transform offDutyPoint, offDutyTeleportPoint;

    bool reachedOff = false;

    public OffDutyState(Transform offDutyPoint, Transform offDutyTeleportPoint, Transform npc)
    {
        
        this.offDutyPoint = offDutyPoint;
        this.offDutyTeleportPoint = offDutyTeleportPoint;
        stateID = FSMStateID.OffDuty;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
        timer = -1;
        
        

    }

    public override void Reason(Transform player, Transform npc)
    {
        
        if(offDutyTanks.Count >= 4 && !offDutyTanks.Contains(npc))
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.ReturnToDuty);
            Debug.Log("Too many off duty.");
        }
        

    }

    public override void Act(Transform player, Transform npc)
    {
        if (!offDutyTanks.Contains(npc) && offDutyTanks.Count < 4)
        {
            offDutyTanks.Add(npc);
            
        }


        Debug.Log("Off Duty");
        if (!reachedOff)
        {
            //2. Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(offDutyPoint.position - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //3. Go Forward
            npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
            timer -= Time.deltaTime;
            //Debug.Log(Vector3.Distance(offDutyPoint.position, npc.position));
            if(Vector3.Distance(offDutyPoint.position, npc.position) < 100f)
            {
                
                npc.position = offDutyTeleportPoint.position;
                reachedOff = true;
                npc.GetComponent<NPCTankController>().StartCoroutine(ReturnToDuty(npc));
            }
        }
        
    }

    IEnumerator ReturnToDuty(Transform npc)
    {
        yield return new WaitForSeconds(10f);
        

        npc.position = offDutyPoint.position;
        npc.GetComponent<NPCTankController>().SetTransition(Transition.ReturnToDuty);

    }
}