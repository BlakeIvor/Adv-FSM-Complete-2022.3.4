using UnityEngine;
using System.Collections;

public class Touch : Sense
{
    [SerializeField] GameObject detectedUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            detectedUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        detectedUI.SetActive(false);
    }


}
