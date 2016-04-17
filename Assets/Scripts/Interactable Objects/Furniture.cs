using UnityEngine;
using System.Collections;

public class Furniture : MonoBehaviour
{
    public GameObject model;
    public Sprite icon;
    public Vector3 rotationModifier;
    public Vector3 positionModifier;

    public Furniture AddFurniture()
    {
        return this;
    }
}
