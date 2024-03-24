using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 1f;
	public float gridSize = 1f;

	[SerializeField] private Sprite upSprite;
	[SerializeField] private Sprite downSprite;
	[SerializeField] private Sprite leftSprite;


	[HideInInspector] public Vector3 targetPosition;
	[HideInInspector] public bool isMoving = false;
	private int electrocution = 1;
	private SpriteRenderer spriteRenderer;

	public static PlayerController instance;
	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		targetPosition = transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
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
				if (movement.x < 0) // Vers la gauche
				{
					spriteRenderer.sprite = leftSprite;
					spriteRenderer.flipX = false;
				}
				else if (movement.x > 0) // Vers la droite
				{
					spriteRenderer.flipX = true;
					spriteRenderer.sprite = leftSprite;
				}
				else if (movement.y > 0) // Vers le bas
				{
					spriteRenderer.flipX = false;
					spriteRenderer.sprite = downSprite;
				}
				else if (movement.y < 0) // Vers le haut
				{
					spriteRenderer.flipX = false;
					spriteRenderer.sprite = upSprite;
				}

				targetPosition = transform.position + RoundVector(movement.normalized * gridSize);
				switch (DatasEnvironement.Instance.tilesDatas[GetPosOnMap()].type)
				{
					case 4:
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
					case 30:
					case 31:
					case 32:
					case 33: 
						AudioManager.instance.PlayConcrete();
						break;
					case 1:
					case 2:
						AudioManager.instance.PlayDirt();
						break;
					case 3:
					case 10:
					case 11:
					case 12:
					case 13:
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

		if (DatasEnvironement.Instance.IsWater(GetPosOnMap()) || DatasEnvironement.Instance.tilesDatas[GetPosOnMap()].type == 3)
		{
			if (DatasEnvironement.Instance.IsWater(GetPosOnMap()))
				Thinking(noWater);
			else
				Thinking("C'est une propriété privée, je ne peux pas passer par ici.");
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
		// Arrondir chaque composante du vecteur � la grille
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
