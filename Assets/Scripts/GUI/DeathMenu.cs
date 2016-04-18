using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathPanel;
    public bool dead = false;
    public string[] deathStrings;

    private Text deathText;
    
    void Start()
    {
        deathText = deathPanel.transform.GetChild(0).GetComponent<Text>();
        deathPanel.SetActive(false);
    }
    
    void Update()
    {
        if(dead)
        {
            int index = Random(0, deathString.length - 1);
            
            deathText = deathString[index];
        }
    }
    
    public void SwitchScene(int sceneIndex)
    {
        Application.loadLevel(sceneIndex);
    }
}
