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

    private int selected = 0;
    
    private List<Transform> objectsDisplay;

    private List<GameObject> objects;

    private RectTransform panelRect;

    private int nbObjects = 0;
    
	void Start ()
    {
        objectsDisplay = new List<Transform>();

        objects = new List<GameObject>();

        panelRect = GetComponent<RectTransform>();

	    for(int i = 1; i < transform.childCount; i++)
        {
            objectsDisplay.Add(transform.GetChild(i));
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
        }
        else
        {
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, new Vector2(80.0f, 0.0f), lerpSpeed * Time.deltaTime);
            transform.GetChild(0).GetComponent<Image>().sprite = openHandle;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(activated)
            {
                activated = false;
            }
            else
            {
                activated = true;
            }
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

        if(nbObjects < 5)
        {
            objectsDisplay[nbObjects].GetComponent<Image>().sprite = icon;
            objects.Add(prefab);
            nbObjects++;
        }
        else
        {
            for(int i = objects.Count - 1; i >= 1; i--)
            {
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
