using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public float sensitivity = 5.0f;
    public float xClamp = 60.0f;

    private float xRotation = 75.0f;
    private float yRotation = 180.0f;

    void Start ()
    {
	    
	}
	
	void Update ()
    {
        yRotation += Input.GetAxis("Mouse X") * sensitivity;

        xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
        xRotation = Mathf.Clamp(xRotation, 0.0f, xClamp);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);

        RaycastHit hit;

        Ray ray = new Ray(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("" + hit.collider.name);

            if(hit.collider.transform.tag != "Player")
            {
                Material startMat = hit.collider.transform.GetComponent<Material>();
                startMat.color = new Color(startMat.color.r, startMat.color.g, startMat.color.b, 0.5f);
            }
        }
    }
}
