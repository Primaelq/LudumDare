using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    private Transform pivot;

	void Start ()
    {
        pivot = transform.GetChild(0);
	}
	
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            if(transform.rotation.x <= 0.0f)
            {
                transform.RotateAround(new Vector3(0.5f, 1.0f, 0.0f), 90.0f);
            }
        }
    }
}
