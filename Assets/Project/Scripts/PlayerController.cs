using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 1f; // Vitesse de d�placement du personnage
	public float gridSize = 1f; // Taille de chaque case de la grille
	[HideInInspector] public Vector3 targetPosition; // Position cible pour le mouvement
	private bool isMoving = false; // Indicateur si le personnage est en mouvement

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
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");
			Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
			if (movement != Vector3.zero)
			{
				targetPosition = transform.position + RoundVector(movement.normalized*gridSize);
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

		while (transform.position != targetPosition)
		{
			Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			transform.position = newPosition;
			yield return null;
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

}
