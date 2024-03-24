using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject Menu_Canvas;
    public GameObject Contexte_Canvas;
    public GameObject Regles_Canvas;
    public GameObject Credits_Canvas;


    public void Menu(){
        Menu_Canvas.SetActive(true);
        Contexte_Canvas.SetActive(false);
        Regles_Canvas.SetActive(false);
        Credits_Canvas.SetActive(false);
    }


    public void Contexte(){
        Menu_Canvas.SetActive(false);
        Contexte_Canvas.SetActive(true);
        Regles_Canvas.SetActive(false);
        Credits_Canvas.SetActive(false);
    }


    public void Regles(){
        Menu_Canvas.SetActive(false);
        Regles_Canvas.SetActive(true);
        Contexte_Canvas.SetActive(false);
        Credits_Canvas.SetActive(false);

    }


    public void Credits(){
        Menu_Canvas.SetActive(false);
        Credits_Canvas.SetActive(true);
        Contexte_Canvas.SetActive(false);
        Regles_Canvas.SetActive(false);

    }


    public void Jouer(){
        SceneManager.LoadScene("SceneDevAnneSo");
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Jouer();
        }
    }
}
