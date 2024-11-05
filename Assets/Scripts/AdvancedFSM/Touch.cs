using UnityEngine;
using System.Collections;

public class Touch : Sense
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player Detected");
        }
    }


}
