using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    
    public bool dead = false;

    
    void Start()
    {
    }
    
    void Update()
    {
        if(dead)
        {
        }
    }
    
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
