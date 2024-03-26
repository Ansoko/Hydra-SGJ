using UnityEngine;

public class GrowAnimation : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private float size;

	private Vector3 initScale = Vector3.one;
	private void Start()
	{
		initScale = transform.localScale;
	}

	void Update()
	{
		transform.localScale = initScale * size + (Vector3.one * Mathf.Cos(Time.time * speed));
	}
}