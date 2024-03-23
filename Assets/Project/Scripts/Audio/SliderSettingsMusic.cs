using UnityEngine;
using UnityEngine.UI;

public class SliderSettingsMusic : MonoBehaviour
{
	private enum Type { Music = 0, SFX = 1 }
	[SerializeField] private Type type = Type.Music;
    [SerializeField] private Slider thisSlider;

	private float coeff;

	private void Awake()
	{
		if(thisSlider==null) thisSlider = GetComponent<Slider>();
	}

	void Start()
    {
		coeff = thisSlider.maxValue - thisSlider.minValue;
        switch (type)
        {
            case Type.Music:
				thisSlider.value = PlayerPrefs.GetFloat("MusicVolume", .5f) * coeff;
				break;
            
            case Type.SFX:
				thisSlider.value = PlayerPrefs.GetFloat("SFXVolume", .5f) * coeff;
				break;
        }
		thisSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
	}

	// Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
		switch (type)
		{
			case Type.Music:
				AudioManager.instance.SetMusicVolume(thisSlider.value/coeff);
				break;

			case Type.SFX:
				AudioManager.instance.SetSFXVolume(thisSlider.value / coeff);
				break;
		}
		
	}
}
