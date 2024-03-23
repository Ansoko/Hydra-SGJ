using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJ : MonoBehaviour
{

    public Canvas dialogueCanvas;

    void Start(){
        dialogueCanvas.gameObject.SetActive(false);
    }
}
