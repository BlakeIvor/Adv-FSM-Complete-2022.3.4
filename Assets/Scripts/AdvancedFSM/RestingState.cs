using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class RestingState : FSMState
{
    
    Transform restPoint;
    float elapsedTime = 0;
    
    public RestingState(Transform npc)
    {
        restPoint = GameObject.FindGameObjectWithTag("RechargeStation").transform;
        

        stateID = FSMStateID.Resting;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

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
        
        if (Vector3.Distance(restPoint.position, npc.position) > 100f)
        {
            Debug.Log("Go to rest");
            destPos = restPoint.position;
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.localPosition);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
            Debug.Log("Translating");
        }
        else
        {
            Healing(npc);
        }
        elapsedTime += Time.deltaTime;
    }

    private void Healing(Transform npc)
    {
        
        Debug.Log("Heal");


        if (elapsedTime >= 3)
        {
            npc.GetComponent<NPCTankController>().health += 30;
            npc.GetComponent<NPCTankController>().healthBar.localScale = new Vector3(npc.GetComponent<NPCTankController>().health / 100.0f * npc.GetComponent<NPCTankController>().healthScale, 14f, 1f);
            elapsedTime = 0.0f;
        }

    }
}
