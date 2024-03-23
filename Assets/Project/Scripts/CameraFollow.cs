using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target; // R�f�rence au transform du joueur � suivre
	public Vector3 offset = new Vector3(0, 0, -10); // D�calage par rapport au joueur
	public float smoothSpeed = 0.125f; // Vitesse de l'effet de glissement

	private Vector3 velocity = Vector3.zero; // V�locit� initiale pour SmoothDamp

	private void LateUpdate()
	{
		if (target != null)
		{
			// Calculer la position cible de la cam�ra avec effet de glissement
			Vector3 desiredPosition = target.position + offset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

			// D�finir la position de la cam�ra
			transform.position = smoothedPosition;
		}
	}
}
