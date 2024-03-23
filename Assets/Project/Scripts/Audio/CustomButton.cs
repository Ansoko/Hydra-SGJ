using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
	public bool isBackButton = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (isBackButton) AudioManager.instance.PlayUIBackEvent();
		else AudioManager.instance.PlayUISelectEvent();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		AudioManager.instance.PlayUIHoverEvent();
	}

}