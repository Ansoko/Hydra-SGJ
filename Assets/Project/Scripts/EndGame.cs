using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject windowCredits;
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void Quitter()
	{
		SceneManager.LoadScene("Scene_Menu");
	}
    public void OpenCredits()
    {
        windowCredits?.SetActive(true);
    }
    public void CloseCredits()
    {
		windowCredits?.SetActive(false);
    }
}
