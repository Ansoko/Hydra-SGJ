using System;
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
	[SerializeField] private GameObject flag;

	private int currentToolIndex = 0;

	public static Inventory Instance;
	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		SelectTool(currentToolIndex);
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
			/*else if (Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				SelectTool(i);
			}else if (Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				SelectTool(i);
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
				if (!DatasEnvironement.Instance.tilesDatas[pos].conductRevealed && !DatasEnvironement.Instance.tilesDatas[pos].sandRevealed) //place flag
				{
					Instantiate(flag, PlayerController.instance.targetPosition, Quaternion.identity, DatasEnvironement.Instance.FlagsParent);
				}
				EM31(pos);
				break;
			
			case 1: //tariere
				if (!DatasEnvironement.Instance.tilesDatas[pos].conductRevealed && !DatasEnvironement.Instance.tilesDatas[pos].sandRevealed) //place flag
				{
					Instantiate(flag, PlayerController.instance.targetPosition, Quaternion.identity, DatasEnvironement.Instance.FlagsParent);
				}
				Tariere(pos);
				break;
		}
	}
	private float minConduct = 4;
	private float maxConduct = 48;
	//pitch entre 0 et 2
	private void EM31(Vector2 pos)
	{
		DatasEnvironement.Instance.RevealConduct(pos);
		float value = DatasEnvironement.Instance.GetConductValue(pos);
		AudioManager.instance.PlayRadar(minConduct * value / maxConduct);
		ShowFlag(pos);
	}
	private void Tariere(Vector2 pos)
	{
		DatasEnvironement.Instance.RevealSand(pos);
		ShowFlag(pos);
	}

	//Flag
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
			sandText.text = "Sable\r\n" + DatasEnvironement.Instance.GetSandValue(pos).ToString() + "m";
		}
	}
}
