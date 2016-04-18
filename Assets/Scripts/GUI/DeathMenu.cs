using UnityEngine;
using UnityEngine.SceneManagement;
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
            int index = Random.Range(0, deathStrings.Length - 1);
            
            deathText.text = deathStrings[index];
        }
    }
    
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
