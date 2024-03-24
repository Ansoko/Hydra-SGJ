using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJ : MonoBehaviour
{

    public Canvas dialogueCanvasOuest;
    public Canvas dialogueCanvasMaison;
    public Canvas dialogueCanvasCentre;
    public Canvas dialogueCanvasSud;

    void Start(){
        dialogueCanvasOuest.gameObject.SetActive(false);
        dialogueCanvasMaison.gameObject.SetActive(false);
        dialogueCanvasCentre.gameObject.SetActive(false);
        dialogueCanvasSud.gameObject.SetActive(false);
    }
}
