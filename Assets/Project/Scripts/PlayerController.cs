using System.Collections;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 1f; // Vitesse de déplacement du personnage
	public float gridSize = 1f; // Taille de chaque case de la grille
	[HideInInspector] public Vector3 targetPosition; // Position cible pour le mouvement
	[HideInInspector] public bool isMoving = false; // Indicateur si le personnage est en mouvement
	private int electrocution = 1;

	public static PlayerController instance;
	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		targetPosition = transform.position;
	}
	private void Update()
	{
		if (!isMoving)
		{
			float moveHorizontal = Input.GetAxis("Horizontal") * electrocution;
			float moveVertical = Input.GetAxis("Vertical") * electrocution;
			Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
			if (movement != Vector3.zero)
			{
				targetPosition = transform.position + RoundVector(movement.normalized * gridSize);
				switch (DatasEnvironement.Instance.tilesDatas[GetPosOnMap()].type)
				{
					case 4:
					case 30:
						AudioManager.instance.PlayConcrete();
						break;
					case 1:
					case 2:
						AudioManager.instance.PlayDirt();
						break;
					case 3:
					case 10:
						AudioManager.instance.PlayGrass();
						break;
				}
				StartCoroutine(MoveToTarget());
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				Inventory.Instance.DoAction(GetPosOnMap());
			}
			/*
			else if (Input.GetKeyUp(KeyCode.E))
			{
				AudioManager.instance.StopRadar();
			}
			*/
		}
	}

	private IEnumerator MoveToTarget()
	{
		isMoving = true;
		Inventory.Instance.HideFlag();
		Vector3 oldPos = transform.position;

		while (transform.position != targetPosition)
		{
			Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			transform.position = newPosition;
			yield return null;
		}

		if (DatasEnvironement.Instance.IsWater(GetPosOnMap()))
		{
			Thinking(noWater);
			AudioManager.instance.PlayCatNo();
			targetPosition = oldPos;
			while (transform.position != targetPosition)
			{
				Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
				transform.position = newPosition;
				yield return null;
			}
		}
		isMoving = false;

		if (nbrTileEMRemaining > 0) EMWay();
		else
		{
			AudioManager.instance.StopRadar();
			Inventory.Instance.EMRemainingText.gameObject.SetActive(false);
		}
		Inventory.Instance.ShowFlag(GetPosOnMap()); 
	}

	private Vector3 RoundVector(Vector3 vector)
	{
		// Arrondir chaque composante du vecteur à la grille
		float newX = Mathf.Round(vector.x / gridSize) * gridSize;
		float newY = Mathf.Round(vector.y / gridSize) * gridSize;
		float newZ = Mathf.Round(vector.z / gridSize) * gridSize;
		return new Vector3(newX, newY, newZ);
	}

	private Vector2 GetPosOnMap()
	{
		return (transform.position - new Vector3(gridSize/2, gridSize/2, 0))/gridSize;
	}

	private int nbrTileEMRemaining = 0;
	public void EMWay()
	{
		nbrTileEMRemaining = Inventory.Instance.EM31(GetPosOnMap());
	}

	[SerializeField] private TMP_Text thinkingText;
	[SerializeField] private string noWater;
	private Coroutine displayCoroutine;
	public void Thinking(string txt, float duration = 5)
	{
		if (displayCoroutine != null)
		{
			StopCoroutine(displayCoroutine);
		}

		thinkingText.text = txt;

		displayCoroutine = StartCoroutine(ClearTextAfterSeconds(duration));
	}

	private IEnumerator ClearTextAfterSeconds(float duration)
	{
		yield return new WaitForSeconds(duration);
		thinkingText.text = "";
		displayCoroutine = null;
	}

	public IEnumerator Elect(float duration = 5)
	{
		if (electrocution == -1) yield break;

		electrocution = -1;
		yield return new WaitForSeconds(duration);
		electrocution = 1;
	}
}
