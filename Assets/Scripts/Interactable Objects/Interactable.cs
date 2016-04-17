using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    public enum Type
    {
        Door,
        LightSwitch,
    }

    public Type type;

    public bool stateON = false;

    public Light[] lights;

    public void Interact()
    {
        switch (type)
        {
            case Type.Door:
                OpenCloseDoor();
                break;

            case Type.LightSwitch:
                TurnLightOnOff();
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
            for(int i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = false;
            }
            stateON = false;
        }
        else
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = true;
            }
            stateON = true;
        }
    }
}
