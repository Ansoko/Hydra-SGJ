using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PNJManager : MonoBehaviour
{
    public GameObject player;
    public PNJ pNJ_Maison_sud;
    public PNJ pNJ_Ferme_est;
    public PNJ pNJ_Ferme_ouest;
    public PNJ pNJ_Ferme_centre;
    public float minDist2PNJ; // Minimal distance to interact with PNJ
    public float minDistDeactivate; // Minimal distance to desactivate canvas
    public float waitBeforePlayerThink;

	[SerializeField] private string dialoguePnjOuest; //ID 
	[SerializeField] private string dialoguePnjEst1;
	[SerializeField] private string dialoguePnjEst2;
	[SerializeField] private string dialoguePnjSud;
	[SerializeField] private string dialoguePnjCentre;

    [SerializeField] private string playerThinkWPnjOuest; //ID 
	[SerializeField] private string playerThinkWPnjEst1;
	[SerializeField] private string playerThinkWPnjEst2;
	[SerializeField] private string playerThinkWPnjSud;
	[SerializeField] private string playerThinkWPnjCentre;

    private Transform playerTransform;
    private PlayerController playerController;

    private float dist_2_PNJ_Maison_sud;
    private float dist_2_PNJ_Ferme_est;
    private float dist_2_PNJ_Ferme_ouest;
    private float dist_2_PNJ_Ferme_centre;

    private bool ouest_canvas_is_active = false;
    private bool centre_canvas_is_active = false;
    private bool est_canvas_is_active = false;
    private bool maison_canvas_is_active = false;




    // Start is called before the first frame update
    void Start()
    {
        // Get the transform component of the player object
        playerTransform = player.GetComponent<Transform>();
        playerController = player.GetComponent<PlayerController>();


        // ID of dialogues
        // dialogueOuest = dialogueOuest.GetDialogueById("10000");
    }

    // Update is c_a_lled once per frame
    void Update()
    {
        // Compute distances between player and PNJ
        dist_2_PNJ_Maison_sud = Distance2PNJ(player, pNJ_Maison_sud.gameObject);
        dist_2_PNJ_Ferme_est = Distance2PNJ(player, pNJ_Ferme_est.gameObject);
        dist_2_PNJ_Ferme_ouest = Distance2PNJ(player, pNJ_Ferme_ouest.gameObject);
        dist_2_PNJ_Ferme_centre = Distance2PNJ(player, pNJ_Ferme_centre.gameObject);
        
        // Ferme Ouest
        if(!ouest_canvas_is_active && dist_2_PNJ_Ferme_ouest < minDist2PNJ)
        {
            AudioManager.instance.PlayCat();
            // pNJ_Ferme_ouest.dialogueCanvasOuest.gameObject.SetActive(true);
            pNJ_Ferme_ouest.dialogueBubble.InitOneDialogue(dialoguePnjOuest);
            ouest_canvas_is_active = true;
            
            // Player Thinking
            PlayerThink(playerThinkWPnjOuest);
        
        }
        if (ouest_canvas_is_active && dist_2_PNJ_Ferme_ouest > minDistDeactivate) 
        {
            pNJ_Ferme_ouest.dialogueBubble.HideDialog();
            ouest_canvas_is_active = false;
        }

        // Maison Sud
        if(!maison_canvas_is_active && dist_2_PNJ_Maison_sud < minDist2PNJ)
        {
            pNJ_Maison_sud.dialogueBubble.InitOneDialogue(dialoguePnjSud);
            maison_canvas_is_active = true;

            // Player Thinking
            PlayerThink(playerThinkWPnjSud);
        }
        if (maison_canvas_is_active == true && dist_2_PNJ_Maison_sud > minDistDeactivate) 
        {
            pNJ_Maison_sud.dialogueBubble.HideDialog();
            maison_canvas_is_active = false;
        }
        
        // Ferme Est
        if(!est_canvas_is_active && dist_2_PNJ_Ferme_est < minDist2PNJ)
        {
            pNJ_Ferme_est.dialogueBubble.InitOneDialogue(dialoguePnjEst1);
            est_canvas_is_active = true;

            // Player Thinking
            PlayerThink(playerThinkWPnjEst1);
        }
        if (est_canvas_is_active == true && dist_2_PNJ_Ferme_est > minDistDeactivate)
        {
            pNJ_Ferme_est.dialogueBubble.HideDialog();
            est_canvas_is_active = false;
        }

        // Ferme Centre
        if(!centre_canvas_is_active && dist_2_PNJ_Ferme_centre < minDist2PNJ)
        {
            pNJ_Ferme_centre.dialogueBubble.InitOneDialogue(dialoguePnjCentre);
            centre_canvas_is_active = true;
            // Player Thinking
            PlayerThink(playerThinkWPnjCentre); 
        }
        if (centre_canvas_is_active == true && dist_2_PNJ_Ferme_centre > minDistDeactivate) 
        {
            pNJ_Ferme_centre.dialogueBubble.HideDialog();
            centre_canvas_is_active = false;
        }
        
    }

    float Distance2PNJ(GameObject object_1, GameObject object_2){
        return Vector3.Distance(object_1.transform.position, object_2.transform.position);
    }

    void PlayerThink(string dialogueText){
        // Call PlayerController.think after a 
        StartCoroutine(PlayerThinkCoroutine(dialogueText));
    }

    IEnumerator PlayerThinkCoroutine(string dialogueText){
        yield return new WaitForSeconds(waitBeforePlayerThink);
        playerController.Thinking(dialogueText);
    }

}

