using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target; // Référence au transform du joueur à suivre
	public Vector3 offset = new Vector3(0, 0, -10); // Décalage par rapport au joueur
	public float smoothSpeed = 0.125f; // Vitesse de l'effet de glissement

	private Vector3 velocity = Vector3.zero; // Vélocité initiale pour SmoothDamp

	private void LateUpdate()
	{
		if (target != null)
		{
			// Calculer la position cible de la caméra avec effet de glissement
			Vector3 desiredPosition = target.position + offset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

			// Définir la position de la caméra
			transform.position = smoothedPosition;
		}
	}
}
