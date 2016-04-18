using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    public GameObject objectsPanel, safePanel, deathPanel;

    public string[] deathStrings;

    public Vector3 thirdPersonViewPos;
    public Vector3 thirdPersonViewRot;

    //public GameObject shapeShiftExplainText;

    public string password;

    public AudioClip selectFurniture;
    public AudioClip ded;
    private AudioSource sound;

    private Vector3 movement;

    private float xRotation = 0.0f;

    private ObjectsManager objManager;

	private Vector3 originalCamPosition;

    private bool lerping = false;

    [HideInInspector]
    public bool openingSafe = false;

    private float lerpSmooth = 0.0f;

    private NavMeshObstacle navMeshObstacle;

    private bool dead;

    private Text deathText;

    void Start ()
    {
        //safePanel.SetActive(false);

        dead = false;

        sound = GetComponent<AudioSource>();

		originalCamPosition = Camera.main.transform.localPosition;

        Cursor.visible = false;

        objManager = objectsPanel.GetComponent<ObjectsManager>();

        navMeshObstacle = GetComponent<NavMeshObstacle>();

        deathText = deathPanel.transform.GetChild(0).GetComponent<Text>();
        deathPanel.SetActive(false);
    }
	
    public void Die()
    {
        if (!dead)
        {
            Cursor.visible = true;

            sound.clip = ded;
            sound.Play();
            while(transform.rotation.eulerAngles.z < 90)
            {
                transform.localRotation = Quaternion.Lerp(Quaternion.Euler(transform.rotation.eulerAngles), Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 100), 0.1f * Time.deltaTime);
            }

            this.enabled = false;
            dead = true;
            objManager.activated = false;
            gameObject.tag = "fml";

            int index = Random.Range(0, deathStrings.Length - 1);
            deathText.text = deathStrings[index];
            deathPanel.SetActive(true);
        }
    }

	void Update ()
    {
        if(!openingSafe)
        {
            if (!shapeShifted)
            {
                //navMeshObstacle.carving = true;
                //shapeShiftExplainText.SetActive(false);
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
                navMeshObstacle.size = objManager.objects[objManager.selected].GetComponent<Collider>().bounds.size;
                navMeshObstacle.carving = true;

                if (!lerping)
                {
                    //shapeShiftExplainText.SetActive(true);
                    objManager.activated = false;

                    if (Vector3.Distance(Camera.main.transform.position, transform.position + thirdPersonViewPos) > 0.01f)
                    {
                        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + thirdPersonViewPos, camLerpSpeed * lerpSmooth);
                        //Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(Camera.main.transform.rotation.eulerAngles), Quaternion.Euler(thirdPersonViewRot), camLerpSpeed * lerpSmooth);
                        Camera.main.transform.LookAt(transform.position);
                        lerpSmooth += Time.deltaTime;
                    }
                    else if (!Camera.main.GetComponent<ThirdPersonCamera>().active)
                    {
                        lerpSmooth = 0.0f;
                        Camera.main.GetComponent<ThirdPersonCamera>().SetNewRotation();
                        Camera.main.GetComponent<ThirdPersonCamera>().active = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space) || lerping)
                {
                    Camera.main.GetComponent<ThirdPersonCamera>().active = false;

                    lerping = true;

                    Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, originalCamPosition, camLerpSpeed * lerpSmooth);
                    Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(Camera.main.transform.rotation.eulerAngles), Quaternion.Euler(18.5f, 0.0f, 0.0f), camLerpSpeed * lerpSmooth);

                    lerpSmooth += Time.deltaTime;

                    if (Vector3.Distance(Camera.main.transform.localPosition, originalCamPosition) < 0.01f)
                    {
                        lerpSmooth = 0.0f;

                        Camera.main.transform.localPosition = originalCamPosition;
                        Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);

                        GetComponent<MeshRenderer>().enabled = true;

                        navMeshObstacle.carving = false;

                        shapeShifted = false;

                        lerping = false;
                    }
                }
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
                    if (furn != null)
                    {
                        objManager.AddObject(furn.model, furn.icon);
                        sound.clip = selectFurniture;
                        sound.Play();

                    }
                }
            }

            if (hit.distance < playerInteractDistance && hit.transform.gameObject.tag == "Interactable")
            {
                if(Input.GetMouseButtonDown(0))
                {
                    hit.transform.gameObject.GetComponent<Interactable>().Interact();
                }
            }

            if (hit.distance < playerInteractDistance && hit.transform.gameObject.tag == "Safe")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OpenSafe();
                    openingSafe = true;
                }
            }
        }

		if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.visible = true;

    }

    void OpenSafe()
    {
        Cursor.visible = true;
        safePanel.SetActive(true);
    }

    public void Submit()
    {
        if(safePanel.transform.GetChild(0).transform.GetComponent<InputField>().text == password)
        {
            Debug.Log("You won");
        }
    }

    public void loadScene(string sceneName)
    {
        if(sceneName == "Current")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

    }

    public void Exit()
    {
        openingSafe = false;
        safePanel.SetActive(false);
        Cursor.visible = false;
    }

    void OnGUI()
    {
        if(!openingSafe || !dead)
        {
            float xMin = (Screen.width / 2) - (crossHair.width / 2);
            float yMin = (Screen.height / 2) - (crossHair.height / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crossHair.width, crossHair.height), crossHair);
        }
    }
}
