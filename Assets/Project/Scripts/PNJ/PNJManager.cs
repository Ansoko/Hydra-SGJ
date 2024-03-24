using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PNJManager : MonoBehaviour
{
    public GameObject player;
    public PNJ pNJ_Maison;
    public PNJ pNJ_Ferme_sud;
    public PNJ pNJ_Ferme_ouest;
    public PNJ pNJ_Ferme_centre;
    public float minDist2PNJ = 2; // Minimal distance to interact with PNJ
    public float minDistDeactivate = 4; // Minimal distance to desactivate canvas

    private Transform playerTransform;
    private float dist_2_PNJ_Maison;
    private float dist_2_PNJ_Ferme_sud;
    private float dist_2_PNJ_Ferme_ouest;
    private float dist_2_PNJ_Ferme_centre;

    private bool ouest_canvas_is_active = false;
    private bool centre_canvas_is_active = false;
    private bool sud_canvas_is_active = false;
    private bool maison_canvas_is_active = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the transform component of the player object
        playerTransform = player.GetComponent<Transform>();
    }

    // Update is c_a_lled once per frame
    void Update()
    {
        // Compute distances between player and PNJ
        dist_2_PNJ_Maison = Distance2PNJ(player, pNJ_Maison.gameObject);
        dist_2_PNJ_Ferme_sud = Distance2PNJ(player, pNJ_Ferme_sud.gameObject);
        dist_2_PNJ_Ferme_ouest = Distance2PNJ(player, pNJ_Ferme_ouest.gameObject);
        dist_2_PNJ_Ferme_centre = Distance2PNJ(player, pNJ_Ferme_centre.gameObject);
        
        // Ferme Ouest
        if(dist_2_PNJ_Ferme_ouest < minDist2PNJ)
        {
            pNJ_Ferme_ouest.dialogueCanvasOuest.gameObject.SetActive(true);
            ouest_canvas_is_active = true;
        }
        if (ouest_canvas_is_active == true && dist_2_PNJ_Ferme_ouest > minDistDeactivate) 
        {
            pNJ_Ferme_ouest.dialogueCanvasOuest.gameObject.SetActive(false);
            ouest_canvas_is_active = false;
        }

        // Maison
        if(dist_2_PNJ_Maison < minDist2PNJ)
        {
            pNJ_Maison.dialogueCanvasMaison.gameObject.SetActive(true);
            maison_canvas_is_active = true;  // should we create other booleans for each PNJ's canvas ? 
        }
        if (maison_canvas_is_active == true && dist_2_PNJ_Maison > minDistDeactivate) 
        {
            pNJ_Maison.dialogueCanvasMaison.gameObject.SetActive(false);
            maison_canvas_is_active = false;
        }
        
        // Ferme Sud
        if(dist_2_PNJ_Ferme_sud < minDist2PNJ)
        {
            pNJ_Ferme_sud.dialogueCanvasSud.gameObject.SetActive(true);
            sud_canvas_is_active = true;  // should we create other booleans for each PNJ's canvas ? 
        }
        if (sud_canvas_is_active == true && dist_2_PNJ_Ferme_sud > minDistDeactivate)
        {
            pNJ_Ferme_sud.dialogueCanvasSud.gameObject.SetActive(false);
            sud_canvas_is_active = false;
        }

        // Ferme Centre
        if(dist_2_PNJ_Ferme_centre < minDist2PNJ)
        {
            pNJ_Ferme_centre.dialogueCanvasCentre.gameObject.SetActive(true);
            centre_canvas_is_active = true;  // should we create other booleans for each PNJ's canvas ? 
        }
        if (centre_canvas_is_active == true && dist_2_PNJ_Ferme_centre > minDistDeactivate) 
        {
            pNJ_Ferme_centre.dialogueCanvasCentre.gameObject.SetActive(false);
            centre_canvas_is_active = false;
        }
        
    }

    float Distance2PNJ(GameObject object_1, GameObject object_2){
        return Vector3.Distance(object_1.transform.position, object_2.transform.position);
    }

}

