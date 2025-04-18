using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public GameObject credits, backButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggleCredits()
    {
        credits.SetActive(!credits.activeSelf);
        if (credits.activeSelf)
        {
            backButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            backButton.GetComponent<Button>().interactable = true;
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
