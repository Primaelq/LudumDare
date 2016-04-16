using UnityEngine;
using System.Collections;

public class WindowSwitch : MonoBehaviour {

	public GameObject someSwitch;
	public GameObject text;

	void Update()
	{
		if(someSwitch.activeInHierarchy)
		{
			text.SetActive(false);
		}
		else
		{
			text.SetActive(true);
		}
	}
}
