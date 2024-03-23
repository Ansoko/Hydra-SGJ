
using System.Collections.Generic;
using System.Xml.Schema;
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

	private int currentToolIndex = 0;

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
		SelectTool(currentToolIndex);
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
				ChangePortefeuille(-priceEM);
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
		tryRemainingEM--;
		EMRemainingText.text = tryRemainingEM.ToString() + "/"+nbrTilesEM;
		PlaceFlag(pos);
		DatasEnvironement.Instance.RevealConduct(pos);
		float value = DatasEnvironement.Instance.GetConductValue(pos);
		AudioManager.instance.PlayRadar(minConduct * value / maxConduct);
		ShowFlag(pos);
		return tryRemainingEM;
	}
	private void Tariere(Vector2 pos)
	{
		ChangePortefeuille(-priceTariere);

		switch(DatasEnvironement.Instance.GetAlea(pos)) { //aléa !!
			case 10:
				PlayerController.instance.Thinking("Je ne peux pas creuser dans la route.");
				AudioManager.instance.PlayCatNo();
				return;
			case 20:
				PlayerController.instance.Thinking("Gzzzt MIAOU ça, c'était un cable éléctrique souterrain !");
				StartCoroutine(PlayerController.instance.Elect());
				AudioManager.instance.PlayCatNo();
				return;
			case 30:
				PlayerController.instance.Thinking("Impossible de creuser ici, on dirait qu'il y a des graviers compacté là dessous !");
				AudioManager.instance.PlayCatNo();
				return;
		}

		PlaceFlag(pos);
		DatasEnvironement.Instance.RevealSand(pos);
		ShowFlag(pos);
	}

	private void Datation(Vector2 pos)
	{
		ChangePortefeuille(-priceDatation);
		ShowFlag(pos);
	}


	//Flag
	private HashSet<Vector2> flagsPlaced = new HashSet<Vector2>();
	public void PlaceFlag(Vector2 pos)
	{
		if (!flagsPlaced.Contains(pos)) //place flag
		{
			flagsPlaced.Add(pos);
			Instantiate(flag, PlayerController.instance.targetPosition, Quaternion.identity, DatasEnvironement.Instance.FlagsParent);
		}
	}
	public void HideFlag()
	{
		drapeauWindow.SetActive(false);
	}
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
						break;
					case 100:
						charbonSable.SetActive(true);
						charbonArgile.SetActive(false);
						break;
					case 101:
						charbonSable.SetActive(true);
						charbonArgile.SetActive(true);
						break;

				}
				
			}

		}
	}
	public void ChangePortefeuille(int price)
	{
		portefeuilleCroquette += price;
		portefeuilleText.text = "Portefeuille : \r\n<b>" +portefeuilleCroquette+"</b> croquettes";
	}
}
