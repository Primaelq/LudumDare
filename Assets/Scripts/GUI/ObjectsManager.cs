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

    private List<Transform> objects;

    private RectTransform panelRect;
    
	void Start ()
    {
        objects = new List<Transform>();

        panelRect = GetComponent<RectTransform>();

	    for(int i = 1; i < transform.childCount; i++)
        {
            objects.Add(transform.GetChild(i));
        }

        DisableExcept(0, objects);
	}
	
	void Update ()
    {
	    if(activated)
        {
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition , new Vector2(- 80.0f, 0.0f), lerpSpeed * Time.deltaTime);
            transform.GetChild(0).GetComponent<Image>().sprite = closeHandle;

            if (Input.GetAxis("Mouse ScrollWheel") < 0 && selected < objects.Count - 1)
            {
                selected++;
                DisableExcept(selected, objects);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0 && selected > 0)
            {
                selected--;
                DisableExcept(selected, objects);
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
