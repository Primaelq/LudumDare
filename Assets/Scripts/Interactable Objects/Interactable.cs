using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    public enum Type
    {
        Door,
        lightSwitch
    }

    public Type type;

    public bool stateON = false;

    public Light light;

    public void Interact()
    {
        switch (type)
        {
            case Type.Door:
                OpenCloseDoor();
                break;

            case Type.lightSwitch:
                break;
        }
    }

    void OpenCloseDoor()
    {
        if(stateON)
        {
            stateON = false;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90.0f, transform.rotation.eulerAngles.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90.0f, transform.rotation.eulerAngles.z);
            stateON = true;
        }
    }

    void TurnLightOnOff()
    {
        if(stateON)
        {
            light.enabled = false;
            stateON = false;
        }
        else
        {
            light.enabled = true;
            stateON = true;
        }
    }
}
