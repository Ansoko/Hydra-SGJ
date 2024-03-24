using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target; // Référence au transform du joueur à suivre
	public Vector3 offset = new Vector3(0, 0, -10); // Décalage par rapport au joueur
	public Vector3 allMapPos = new Vector3(0, 0, -10);
	private bool seeAll = false;
	public float smoothSpeed = 0.125f; // Vitesse de l'effet de glissement

	private Vector3 velocity = Vector3.zero; // Vélocité initiale pour SmoothDamp

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			seeAll = !seeAll;
			AudioManager.instance.PlayWhoosh();
		}
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			Vector3 desiredPosition;
			if (seeAll) desiredPosition = allMapPos;
			else desiredPosition = target.position + offset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

			transform.position = smoothedPosition;
		}
	}
}
