using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public float sensitivity = 5.0f;
    public float xClamp = 60.0f;

    private float xRotation;
    private float yRotation;

    [HideInInspector]
    public bool active = false;

    void Start ()
    {

	}
	
	void Update ()
    {
        if(active)
        {
            yRotation += Input.GetAxis("Mouse X") * sensitivity;

            xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
            xRotation = Mathf.Clamp(xRotation, 0.0f, xClamp);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);

            RaycastHit hit;

            Ray ray = new Ray(transform.position, GameObject.FindGameObjectWithTag("FurnitureShape").transform.position - transform.position);

            Debug.DrawRay(transform.position, GameObject.FindGameObjectWithTag("FurnitureShape").transform.position - transform.position, Color.red);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

                if (hit.collider.transform.tag != "FurnitureShape")
                {
                    transform.Translate(Vector3.forward * 0.1f);
                }
            }
        }
    }

    public void SetNewRotation()
    {
        transform.LookAt(transform.parent.transform.position);
        xRotation = transform.rotation.eulerAngles.x;
        yRotation = transform.rotation.eulerAngles.y;
    }
}
