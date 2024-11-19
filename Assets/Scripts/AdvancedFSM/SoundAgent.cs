using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAgent : MonoBehaviour
{
    private Vector3 destPos;
    private GameObject[] pointList;
    private PlayerTankController player;

    // Start is called before the first frame update
    void Start()
    {
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");
        FindNextPoint();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTankController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, destPos) <= 100.0f)
        {
            FindNextPoint();
        }

        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);  

        transform.Translate(Vector3.forward * Time.deltaTime * 150.0f);
    }

    void FindNextPoint()
    {
        int rndIndex = Random.Range(0, pointList.Length);
        float rndRadius = 10.0f;
        
        Vector3 rndPosition = Vector3.zero;
        destPos = pointList[rndIndex].transform.position + rndPosition;

        //Check Range
        //Prevent to decide the random point as the same as before
        if (IsInCurrentRange(destPos))
        {
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
            destPos = pointList[rndIndex].transform.position + rndPosition;
        }
    }

    bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;

        return false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (player.getCurrSpeed() > 0)
            {
                // Player moving so detect
                Debug.Log("Detected Player movement");
            }
        }
    }
}
