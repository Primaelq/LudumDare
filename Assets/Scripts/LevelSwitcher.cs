using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSwitcher : MonoBehaviour
{
    public float waitTime = 2.0f;
    public int sceneIndex;

	void Start ()
    {
	    
	}
	
	void Update ()
    {
	    
	}

    IEnumerator waitBeforeChangeScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneIndex);
    }
}
