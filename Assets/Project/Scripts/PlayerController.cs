using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 1f; // Vitesse de déplacement du personnage
	public float gridSize = 1f; // Taille de chaque case de la grille
	private Vector3 targetPosition; // Position cible pour le mouvement
	private bool isMoving = false; // Indicateur si le personnage est en mouvement

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
				Debug.Log(DatasEnvironement.Instance.GetSandValue(GetPosOnMap()));
				AudioManager.instance.PlayRadar(DatasEnvironement.Instance.GetSandValue(GetPosOnMap()) - 1);
			}
			else if (Input.GetKeyUp(KeyCode.E))
			{
				AudioManager.instance.StopRadar();
			}
		}
	}

	private IEnumerator MoveToTarget()
	{
		isMoving = true;
		while (transform.position != targetPosition)
		{
			Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			transform.position = newPosition;
			yield return null;
		}
		isMoving = false;
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

}
