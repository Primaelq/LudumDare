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

	public GameObject shapeShiftExplainText;

    private Vector3 movement;


    private float xRotation = 0.0f;

    private ObjectsManager objManager;

    private Mesh defaultMesh;

	private Vector3 originalCamPosition;

    private bool lerping = false;

    private float lerpSmooth = 0.0f;

	void Start ()
    {
		originalCamPosition = Camera.main.transform.localPosition;

        Cursor.visible = false;

        objManager = objectsPanel.GetComponent<ObjectsManager>();

        defaultMesh = GetComponent<MeshFilter>().mesh;

        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
    }
	
	void Update ()
    {
        if(!shapeShifted)
        {
			shapeShiftExplainText.SetActive(false);
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
            if(!lerping)
            {
                shapeShiftExplainText.SetActive(true);
                objManager.activated = false;
                if (Vector3.Distance(Camera.main.transform.position, transform.position + thirdPersonViewPos) > 0.01f)
                {
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + thirdPersonViewPos, camLerpSpeed * lerpSmooth);
                    Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(Camera.main.transform.rotation.eulerAngles), Quaternion.Euler(thirdPersonViewRot), camLerpSpeed * lerpSmooth);
                    lerpSmooth += Time.deltaTime;
                }
                else
                {
                    lerpSmooth = 0.0f;
                    Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
                }
            }

			if(Input.GetKeyDown(KeyCode.Space) || lerping)
            {
                Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;

                lerping = true;

				Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, originalCamPosition, camLerpSpeed * lerpSmooth);
				Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(Camera.main.transform.rotation.eulerAngles), Quaternion.Euler(18.5f, 0.0f, 0.0f), camLerpSpeed * lerpSmooth);

                lerpSmooth += Time.deltaTime;

                if (Vector3.Distance(Camera.main.transform.localPosition, originalCamPosition) < 0.01f)
                {
                    lerpSmooth = 0.0f;

                    Camera.main.transform.localPosition = originalCamPosition;
                    Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);

                    GetComponent<MeshFilter>().mesh = defaultMesh;

                    shapeShifted = false;

                    lerping = false;
                }
            }
            else
            {

            }
        }

        RaycastHit hit;

        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance < playerInteractDistance && hit.transform.gameObject.tag == "Furniture" && !objManager.activated)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Furniture furn = hit.transform.GetComponent<Furniture>().AddFurniture();
                    objManager.AddObject(furn.model, furn.icon);
                }
            }
            if (hit.distance < playerInteractDistance && hit.transform.gameObject.tag == "Interactable")
            {
                if (hit.transform.gameObject.GetComponent<InteractableHighlight>() != null)
                {
                    hit.transform.gameObject.GetComponent<InteractableHighlight>().Hover();
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
