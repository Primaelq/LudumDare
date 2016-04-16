using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float sensitivity = 5.0f;
    public float xClamp = 60.0f;
    public float camLerpSpeed = 5.0f;

    public float playerInteractDistance = 2.0f;

    public Texture crossHair;

    public bool shapeShifted = false;

    public GameObject objectsPanel;

    public Vector3 thirdPersonViewPos;
    public Vector3 thirdPersonViewRot;

    private Vector3 CameraDefaultPos;

    private Vector3 movement;

    private float xRotation = 0.0f;

    private ObjectsManager objManager;

	void Start ()
    {
        Cursor.visible = false;

        objManager = objectsPanel.GetComponent<ObjectsManager>();

        CameraDefaultPos = Camera.main.transform.position;

        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
    }
	
	void Update ()
    {
        if(!shapeShifted)
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
        }
        else
        {
            if (Vector3.Distance(Camera.main.transform.position, thirdPersonViewPos) > 0.5f)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, thirdPersonViewPos, camLerpSpeed * Time.deltaTime);
                Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(Camera.main.transform.rotation.eulerAngles), Quaternion.Euler(thirdPersonViewRot), camLerpSpeed * Time.deltaTime);
            }
            else
            {
                Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
            }
        }

        RaycastHit hit;

        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.distance < playerInteractDistance && hit.transform.gameObject.tag == "Furniture")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Furniture furn = hit.transform.GetComponent<Furniture>().AddFurniture();
                    objManager.AddObject(furn.model, furn.icon);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.visible = true;
    }

    void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crossHair.width / 2);
        float yMin = (Screen.height / 2) - (crossHair.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crossHair.width, crossHair.height), crossHair);
    }
}
