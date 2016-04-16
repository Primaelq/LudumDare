using UnityEngine;
using System.Collections;

public class InteractableHighlight : MonoBehaviour {

    public Material m;

    public Color hover;
    private Color original;
    void Start()
    {
        original = m.color;
    }

    public void Hover()
    {
        CancelInvoke("UnHover");
        GetComponent<Renderer>().material.SetColor("_Color", hover);
        Invoke("UnHover", 0.1f);
    }

    public void UnHover()
    {
        GetComponent<Renderer>().material.SetColor("_Color", original);
    }
}
