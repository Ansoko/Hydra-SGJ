using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJ : MonoBehaviour
{


    public DialogueBubble dialogueBubble;

    void Start(){
        dialogueBubble.gameObject.SetActive(false);
    //dialogueCanvasMaisonSud.gameObject.SetActive(false);
    //dialogueCanvasCentre.gameObject.SetActive(false);
    //    dialogueCanvasEst.gameObject.SetActive(false);
    }
}
