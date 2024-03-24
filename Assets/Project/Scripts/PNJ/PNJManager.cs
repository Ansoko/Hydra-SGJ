using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PNJManager : MonoBehaviour
{
    

    public GameObject player;
    public GameObject pNJ_Maison;
    public GameObject pNJ_Ferme_sud;
    public PNJ pNJ_Ferme_ouest;
    public GameObject pNJ_Ferme_centre;
    
    private Transform playerTransform;

    // Reference of canvas of each PNJ 

    public float minDist2PNJ = 30; // Distance minimale pour intéragir avec les PNJ -> A REDUIRE, check en mode PLAY 
    // Chat = 1 unité Unity 

    // TODO : distance à partir de laquelle on desactive le canvas -> check en mode PLAY

    // Initialize variables
    private float dist_2_PNJ_Maison;
    private float dist_2_PNJ_Ferme_sud;
    private float dist_2_PNJ_Ferme_ouest;
    private float dist_2_PNJ_Ferme_centre;

    // Start is called before the first frame update
    void Start()
    {
        // Get the transform component of the player object
        playerTransform = player.GetComponent<Transform>();
    }



    // Update is c_a_lled once per frame
    void Update()
    {
        
        // Log the position of the player object
        // Debug.Log("Player Position: " + playerTransform.position);
        
        dist_2_PNJ_Maison = Distance2PNJ(player, pNJ_Maison);
        dist_2_PNJ_Ferme_sud = Distance2PNJ(player, pNJ_Ferme_sud);
        dist_2_PNJ_Ferme_ouest = Distance2PNJ(player, pNJ_Ferme_ouest.gameObject);
        dist_2_PNJ_Ferme_centre = Distance2PNJ(player, pNJ_Ferme_centre);

        
        if(dist_2_PNJ_Maison < minDist2PNJ)
        {
            Debug.Log(dist_2_PNJ_Maison);
            // Activate canvas 
            Interact_w_PNJ_Maison();
        }
        
        if(dist_2_PNJ_Ferme_sud < minDist2PNJ)
        {   
            Debug.Log(dist_2_PNJ_Ferme_sud);
            Interact_w_PNJ_Ferme_sud();
        }
        
        if(dist_2_PNJ_Ferme_ouest < minDist2PNJ)
        {
            pNJ_Ferme_ouest.dialogueCanvas.gameObject.SetActive(true);
        }
        

        if(dist_2_PNJ_Ferme_centre < minDist2PNJ)
        {
            Debug.Log(dist_2_PNJ_Ferme_centre);
            Interact_w_PNJ_Ferme_centre();
        }
        

    }

    float Distance2PNJ(GameObject object_1, GameObject object_2){
        return Vector3.Distance(object_1.transform.position, object_2.transform.position);
    }

    void Interact_w_PNJ_Maison(){
        Debug.Log("PNJ Maison");
    }

    void Interact_w_PNJ_Ferme_sud(){
        Debug.Log("PNJ Ferme sud");

    }

    void Interact_w_PNJ_Ferme_ouest(){
        Debug.Log("PNJ Ferme ouest");

    }

    void Interact_w_PNJ_Ferme_centre(){
        Debug.Log("PNJ Ferme centre");

    }
}

