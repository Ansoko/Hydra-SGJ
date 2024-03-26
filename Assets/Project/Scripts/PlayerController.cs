using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 1f;
	public float gridSize = 1f;
	[SerializeField] private GameObject cercleAnimation;

	[HideInInspector] public Vector3 targetPosition;
	[HideInInspector] public bool isMoving = false;
	private int electrocution = 1;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	public static PlayerController instance;
	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		cercleAnimation?.SetActive(false);
		targetPosition = transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
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
					animator.Play("Walk_side");
					spriteRenderer.flipX = false;
				}
				else if (movement.x > 0) // Vers la droite
				{
					animator.Play("Walk_side");
					spriteRenderer.flipX = true;
				}
				else if (movement.y > 0) // Vers le bas
				{
					animator.Play("Walk_back");
					spriteRenderer.flipX = false;
				}
				else if (movement.y < 0) // Vers le haut
				{
					animator.Play("Walk_Front");
					spriteRenderer.flipX = false;
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
			else
			{
				animator.Play("Idle");
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

	[SerializeField] private GameObject textInfo;
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
				Thinking(Dialogue.instance.GetDialogueById("1012"));
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

		textInfo.SetActive(!(GetPosOnMap().x > 36));

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
		
		cercleAnimation.SetActive(true);
		electrocution = -1;
		yield return new WaitForSeconds(duration);
		cercleAnimation.SetActive(false);
		electrocution = 1;
	}
}
