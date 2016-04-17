using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public float sensitivity = 5.0f;
    public float xClamp = 60.0f;

    private float xRotation;
    private float yRotation;

    void Start ()
    {
        transform.LookAt(transform.parent.transform.position);
        xRotation = transform.rotation.eulerAngles.x;
        yRotation = transform.rotation.eulerAngles.y;
        Debug.Log(xRotation);
	}
	
	void Update ()
    {
        yRotation += Input.GetAxis("Mouse X") * sensitivity;

        xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
        xRotation = Mathf.Clamp(xRotation, 0.0f, xClamp);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);

        RaycastHit hit;

        Ray ray = new Ray(transform.position, GameObject.FindGameObjectWithTag("FurnitureShape").transform.position - transform.position);

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.transform.tag != "FurnitureShape")
            {
                Material startMat = hit.collider.transform.GetComponent<Renderer>().material;
                startMat.color = new Color(startMat.color.r, startMat.color.g, startMat.color.b, 0.5f);
            }
        }
    }
}
