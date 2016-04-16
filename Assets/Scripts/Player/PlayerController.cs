using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float sensitivity = 5.0f;
    public float xClamp = 60.0f;

    private Vector3 movement;

    private float xRotation = 0.0f;

	void Start ()
    {
        Cursor.visible = false;
	}
	
	void Update ()
    {
        float yRotation = Input.GetAxis("Mouse X") * sensitivity;

        transform.Rotate(0.0f, yRotation, 0.0f);

        xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

        movement.z = Input.GetAxis("Vertical") * moveSpeed;
        movement.x = Input.GetAxis("Horizontal") * moveSpeed;
        movement.y = 0.0f;

        movement = transform.rotation * movement;

        CharacterController controller = GetComponent<CharacterController>();

        controller.SimpleMove(movement);

        if(Input.GetKeyDown(KeyCode.Escape))
            Cursor.visible = true;
    }
}
