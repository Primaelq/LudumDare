using UnityEngine;
using System.Collections;

public class CheckKillPlayer : MonoBehaviour
{
    public bool playerInRange;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "FML")
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "FML")
            playerInRange = false;
    }
}
