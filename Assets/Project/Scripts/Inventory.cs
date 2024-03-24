
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public GameObject[] tools;

	[SerializeField] private GameObject drapeauWindow;
	[SerializeField] private GameObject conductWindow;
	[SerializeField] private TMP_Text conductText;
	[SerializeField] private GameObject sandWindow;
	[SerializeField] private TMP_Text sandText;
	[SerializeField] private RectTransform sandSlider;
	[SerializeField] private GameObject charbonSable;
	[SerializeField] private GameObject charbonArgile;
	[SerializeField] private GameObject flag;
	[SerializeField] private TMP_Text portefeuilleText;
	public TMP_Text EMRemainingText;
	[SerializeField] private int portefeuilleCroquette = 2000;
	[SerializeField] private int priceEM;
	[SerializeField] private int priceTariere;
	[SerializeField] private int priceDatation;
	[SerializeField] private int nbrTilesEM;

	private int currentToolIndex = -1;

	public static Inventory Instance;
	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		drapeauWindow.SetActive(false);
		conductWindow.SetActive(false);
		sandWindow.SetActive(false);
		//SelectTool(currentToolIndex);
		ChangePortefeuille(0);
	}

	void Update()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f)
		{
			SelectNextTool();
		}
		else if (scroll < 0f)
		{
			SelectPreviousTool();
		}

		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SelectTool(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				SelectTool(1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				SelectTool(2);
			}
			/*else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				SelectTool(3);
			}
			*/
		}
	}

	void SelectNextTool()
	{
		currentToolIndex = (currentToolIndex + 1) % tools.Length;
		UpdateToolSelection();
	}

	void SelectPreviousTool()
	{
		currentToolIndex = (currentToolIndex - 1 + tools.Length) % tools.Length;
		UpdateToolSelection();
	}

	public void SelectTool(int index)
	{
		if (index >= 0 && index < tools.Length)
		{
			currentToolIndex = index;
			UpdateToolSelection();
		}
	}

	void UpdateToolSelection()
	{
		AudioManager.instance.PlayUISelectEvent();

		foreach (var tool in tools)
		{
			tool.SetActive(false);
		}

		tools[currentToolIndex].SetActive(true);
	}

	public void DoAction(Vector2 pos)
	{
		switch (currentToolIndex)
		{
			case 0: //conductivity
				if (tryRemainingEM != 0) break; //encore en cours
				//else
				EMRemainingText.gameObject.SetActive(true);
				tryRemainingEM = nbrTilesEM;
				if (pos.x > 36) ChangePortefeuille(-priceEM);
				PlayerController.instance.EMWay();
				//EM31(pos);
				break;
			
			case 1: //tariere
				Tariere(pos);
				break;


			case 2: //datation
				//add flag ? : PlaceFlag(pos);
				Datation(pos);
				break;
		}
	}
	private float minConduct = 4;
	private float maxConduct = 48*2;
	//pitch entre 0 et 2
	private int tryRemainingEM = 0;
	public int EM31(Vector2 pos)
	{
		AudioManager.instance.PlayEM13();
		tryRemainingEM--;
		EMRemainingText.text = tryRemainingEM.ToString() + "/"+nbrTilesEM;
		DatasEnvironement.Instance.RevealConduct(pos);
		float value = minConduct * DatasEnvironement.Instance.GetConductValue(pos) / maxConduct;
		//Debug.Log(value/2);
		if (value / 2 < 0.5f)
		{
			PlaceFlag(pos, Color.Lerp(Color.blue, Color.yellow, value));
		}
		else
		{
			PlaceFlag(pos, Color.Lerp(Color.yellow, Color.red, ((value/2) - 0.5f)*2));
		}
		AudioManager.instance.PlayRadar(value);
		ShowFlag(pos);
		return tryRemainingEM;
	}
	private void Tariere(Vector2 pos)
	{
		AudioManager.instance.PlayTariere();
		if(pos.x>36) ChangePortefeuille(-priceTariere);

		switch(DatasEnvironement.Instance.GetAlea(pos)) { //aléa !!
			case 10:
				PlayerController.instance.Thinking("Je ne peux pas creuser dans la route.");
				AudioManager.instance.PlayCatNo();
				return;
			case 20:
				PlayerController.instance.Thinking("Gzzzt MIAOU ! Ca, c'était un cable éléctrique souterrain !");
				StartCoroutine(PlayerController.instance.Elect());
				AudioManager.instance.PlayBzz();
				return;
			case 30:
				PlayerController.instance.Thinking("Impossible de creuser ici, on dirait qu'il y a des graviers compacté là dessous !");
				AudioManager.instance.PlayCatNo();
				return;
		}

		PlaceFlag(pos, Color.white);
		DatasEnvironement.Instance.RevealSand(pos);
		ShowFlag(pos);
	}

	private void Datation(Vector2 pos)
	{
		//Debug.Log(DatasEnvironement.Instance.GetCharbValue(pos));
		if (DatasEnvironement.Instance.tilesDatas[pos].sandRevealed)
		{
			AudioManager.instance.PlayC14();
			switch (DatasEnvironement.Instance.GetCharbValue(pos))
			{
				case 0:
					Dialogue.instance.InitOneNewDialogue("Il n'y a pas de charbon dans le sol à analyser.");
					break;
				case 100: //sable 
					Dialogue.instance.InitOneDialogue("1004");
					if (pos.x > 36) ChangePortefeuille(-priceDatation);
					break;
				case 1://argile
				case 101://sable + argile
					//Dialogue.instance.InitOneDialogue("1005");
					//endGame
					EndGame(Dialogue.instance.GetDialogueById("1005"));

					if (pos.x > 36) ChangePortefeuille(-priceDatation);
					break;

			}
			ShowFlag(pos);
		}
		else
		{
			AudioManager.instance.PlayCat();
			Dialogue.instance.InitOneNewDialogue("J'ai besoin de prélever des charbons du sol avec la tarière avant de les analyser.");
		}
	}


	//Flag
	private HashSet<Vector2> flagsPlaced = new HashSet<Vector2>();
	public void PlaceFlag(Vector2 pos, Color couleur)
	{
		if (!flagsPlaced.Contains(pos)) //place flag
		{
			flagsPlaced.Add(pos);
			GameObject flagobj = Instantiate(flag, PlayerController.instance.targetPosition, Quaternion.identity, DatasEnvironement.Instance.FlagsParent);
			flagobj.GetComponent<SpriteRenderer>().color = couleur;
		}
	}
	public void HideFlag()
	{
		drapeauWindow.SetActive(false);
	}

	private bool hasSeenCharbSand = false;
	private bool hasSeenCharbArgile = false;
	public void ShowFlag(Vector2 pos)
	{
		if (DatasEnvironement.Instance.HasBeenPlanted(pos))
		{
			drapeauWindow.SetActive(true);
			conductWindow.SetActive(DatasEnvironement.Instance.tilesDatas[pos].conductRevealed);
			conductText.text = "<b>Avec l'EM31</b>\r\nConductivité : " + DatasEnvironement.Instance.GetConductValue(pos).ToString() + "mS";			
			sandWindow.SetActive(DatasEnvironement.Instance.tilesDatas[pos].sandRevealed);
			if (DatasEnvironement.Instance.tilesDatas[pos].sandRevealed)
			{
				sandText.text = "Sable\r\n" + DatasEnvironement.Instance.GetSandValue(pos).ToString() + "m";
				sandSlider.sizeDelta = new(0, DatasEnvironement.Instance.GetSandValue(pos) * 300 / 4.01f); //4,01m max
				switch (DatasEnvironement.Instance.GetCharbValue(pos)) {
					case 0:
						charbonSable.SetActive(false);
						charbonArgile.SetActive(false);
						break;
					case 1:
						charbonSable.SetActive(false);
						charbonArgile.SetActive(true);
						if (!hasSeenCharbArgile)
						{
							Dialogue.instance.InitOneDialogue("1003");
							hasSeenCharbArgile = true;
						}
						break;
					case 100:
						charbonSable.SetActive(true);
						charbonArgile.SetActive(false);
						if (!hasSeenCharbSand)
						{
							Dialogue.instance.InitOneDialogue("1002");
							hasSeenCharbSand = true;
						}
						break;
					case 101:
						charbonSable.SetActive(true);
						charbonArgile.SetActive(true);
						if (!hasSeenCharbArgile)
						{
							Dialogue.instance.InitOneDialogue("1003");
							hasSeenCharbArgile = true;
						}
						break;

				}
				
			}

		}
	}
	public void ChangePortefeuille(int price)
	{
		portefeuilleCroquette += price;
		portefeuilleText.text = "Portefeuille : \r\n<b>" +portefeuilleCroquette+"</b> croquettes";

		if (portefeuilleCroquette < priceDatation * 2)
		{
			Dialogue.instance.InitOneDialogue("1000");
		}

		if (portefeuilleCroquette < priceDatation) EndGame("C’est fini pour aujourd'hui, je n’ai même plus assez de croquettes pour financer une datation… Il faudra mieux gérer nos ressources la prochaine fois. En attendant, il n'y a plus qu'à chercher de nouveaux financements.");
		if (portefeuilleCroquette < 0) EndGame("Plus de croquette pour continuer les recherches, il faudra mieux gérer nos ressources la prochaine fois... En attendant, il n'y a plus qu'à chercher de nouveaux financements.");
	}


	#region GameOver

	[SerializeField] private GameObject gameOverWindow;
	private void EndGame(string txt)
	{
		AudioManager.instance.StopRadar();
		PlayerController.instance.isMoving = true;
		gameOverWindow.SetActive(true);
		gameOverWindow.GetComponentInChildren<TMP_Text>(true).text = txt;
	}

	#endregion
}
