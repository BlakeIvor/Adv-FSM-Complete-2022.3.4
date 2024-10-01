using UnityEngine;
using System.Collections;

public class RestingState : FSMState
{
    
    Transform restPoint;

    
    public RestingState(Transform npc)
    {
        restPoint = GameObject.FindGameObjectWithTag("RechargeStation").transform;
        

        stateID = FSMStateID.Resting;
        
        
    }

    public override void Reason(Transform player, Transform npc)
    {
        int health = npc.GetComponent<NPCTankController>().health;
        npc.GetComponent<NPCTankController>().health = (int) Mathf.Clamp(health, 0f, 100f);
        if (health >= 100f)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.FullHealth);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        
        if (!(npc.GetComponent<NPCTankController>().inChargingArea))
        {
            destPos = restPoint.position;
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
        }
        else if (npc.GetComponent<NPCTankController>().inChargingArea)
        {
            Healing(npc);
        }
        
    }

    private void Healing(Transform npc)
    {
        npc.GetComponent<NPCTankController>().health += 30;
        Debug.Log("Heal");
         
    }
}
