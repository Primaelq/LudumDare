using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectsManager : MonoBehaviour
{
    public bool activated = false;

    public float lerpSpeed = 5.0f;

    public Sprite openHandle;
    public Sprite closeHandle;

    [HideInInspector]
    public int selected = 0;
    
    private List<Transform> objectsDisplay;

    [HideInInspector]
    public List<GameObject> objects;

    private RectTransform panelRect;

    private int nbObjects = 0;

    public GameObject tempShape;

    public AudioSource playerSoundsrc;
    public AudioClip becomeFurnitureSound;
    public AudioClip becomeHumanSound;


    void Start ()
    {
        objectsDisplay = new List<Transform>();

        objects = new List<GameObject>();

        panelRect = GetComponent<RectTransform>();

        //tempShape = new GameObject();
        //tempShape.SetActive(false);

	    for(int i = 1; i < transform.childCount; i++)
        {
            objectsDisplay.Add(transform.GetChild(i));
			objects.Add(null);
        }

        DisableExcept(0, objectsDisplay);
	}
	
	void Update ()
    {
	    if(activated)
        {
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition , new Vector2(- 80.0f, 0.0f), lerpSpeed * Time.deltaTime);
            transform.GetChild(0).GetComponent<Image>().sprite = closeHandle;

            if (Input.GetAxis("Mouse ScrollWheel") < 0 && selected < objectsDisplay.Count - 1)
            {
                selected++;
                DisableExcept(selected, objectsDisplay);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0 && selected > 0)
            {
                selected--;
                DisableExcept(selected, objectsDisplay);
            }

            if(Input.GetMouseButtonDown(0))
            {

                if (objects[selected] != null)
                {
                    Transform tempTransform = GameObject.FindGameObjectWithTag("Player").transform;
                    Vector3 tempPosition = tempTransform.position + objects[selected].transform.GetComponent<Furniture>().positionModifier;
                    tempTransform.GetComponent<MeshRenderer>().enabled = false;
                    Quaternion rotation = Quaternion.Euler(objects[selected].transform.GetComponent<Furniture>().rotationModifier.x, GameObject.FindGameObjectWithTag("Player").transform.rotation.eulerAngles.y, objects[selected].transform.GetComponent<Furniture>().rotationModifier.z);
                    GameObject.FindGameObjectWithTag("Player").transform.tag = "FurnitureShape";
                    //tempShape.SetActive(true);
                    tempShape = Instantiate(objects[selected], tempPosition, rotation) as GameObject;
                    tempShape.tag = "fml";
                    tempTransform.GetComponent<PlayerController>().shapeShifted = true;
                    //SOUND
                    playerSoundsrc.clip = becomeFurnitureSound;
                    playerSoundsrc.Play();
                }
                else
                    Debug.Log("Object null");
            }
        }
        else
        {
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, new Vector2(80.0f, 0.0f), lerpSpeed * Time.deltaTime);
            transform.GetChild(0).GetComponent<Image>().sprite = openHandle;
        }

        if(Input.GetKeyDown(KeyCode.Space) && GameObject.FindGameObjectWithTag("Player") != null)
        {
            if(activated)
            {
                activated = false;
                Cursor.visible = false;
            }
            else
            {
                activated = true;
                Cursor.visible = false;
            }
        }

        if(GameObject.FindGameObjectWithTag("FurnitureShape") != null && !GameObject.FindGameObjectWithTag("FurnitureShape").transform.GetComponent<PlayerController>().shapeShifted && tempShape.activeSelf)
        {
            GameObject.FindGameObjectWithTag("FurnitureShape").transform.tag = "Player";
           //objects[selected] = tempShape.GetComponent<Furniture>().model;
            //tempShape.SetActive(false);
            Destroy(tempShape);
            tempShape = null;
            Debug.Log(objects[selected] == null);
            // tempShape = new GameObject();
            playerSoundsrc.clip = becomeHumanSound;
            playerSoundsrc.Play();
        }
    }

    public void AddObject(GameObject prefab, Sprite icon)
    {
        for(int i = 0; i < objects.Count; i++)
        {
            if(prefab == objects[i])
            {
                Debug.Log("Item already in the list");
                return;
            }
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if ( objects[i] != null && prefab.name == objects[i].name)
            {
                Debug.Log("Item already in the list");
                return;
            }
        }

        if (nbObjects < 5)
        {
            objectsDisplay[selected].GetComponent<Image>().sprite = icon;
			objects[selected] = prefab;
            //nbObjects++;
        }
        else
        {
            for(int i = objects.Count - 1; i >= 1; i--)
            {
				Debug.Log("I made it");
                objects[i] = objects[i - 1];
                objectsDisplay[i].GetComponent<Image>().sprite = objectsDisplay[i - 1].GetComponent<Image>().sprite;
            }

            objectsDisplay[0].GetComponent<Image>().sprite = icon;
            objects[0] = prefab;
        }
    }

    public void DisableExcept(int index, List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i != index)
            {
                list[i].GetComponent<Image>().color = Color.black;
            }
            else
            {
                list[i].GetComponent<Image>().color = Color.white;
            }
        }
    }
}
