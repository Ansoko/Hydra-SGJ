using UnityEngine;
/// <summary>
/// Used for playing and manage sounds & musics
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioSource sfxAudioSource;
	[SerializeField] private AudioSource radarSource;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
				/*
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		*/

		//audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", .5f);
		//sfxAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", .5f);
	}

	public void PlayMusic(AudioClip audioClip)
	{
		if (audioSource.isPlaying) audioSource.Stop();
		audioSource.PlayOneShot(audioClip);
	}

	public void PlaySFX(AudioClip audioClip)
	{
		if (sfxAudioSource.isPlaying) sfxAudioSource.Stop();
		sfxAudioSource.PlayOneShot(audioClip);
	}

	public bool IsAudioPlaying()
	{
		return audioSource.isPlaying;
	}

	#region Settings
	/// <param name="volume">Between 0 and 1</param>
	public void SetMusicVolume(float volume)
	{
		audioSource.volume = volume;
		PlayerPrefs.SetFloat("MusicVolume", volume);
	}
	/// <param name="volume">Between 0 and 1</param>
	public void SetSFXVolume(float volume)
	{
		sfxAudioSource.volume = volume;
		PlayerPrefs.SetFloat("SFXVolume", volume);
	}
	#endregion


	#region Ui buttons
	[Header("UI buttons sounds")]
	[SerializeField] private AudioClip uiHover;
	[SerializeField] private AudioClip uiSelect;
	[SerializeField] private AudioClip uiBack;
	public void PlayUIHoverEvent()
	{
		if (uiHover != null) PlaySFX(uiHover);
		else Debug.LogWarning("No sound for hovering button");
	}
	public void PlayUISelectEvent()
	{
		if (uiSelect != null) PlaySFX(uiSelect);
		else Debug.LogWarning("No sound for clicking button");
	}
	public void PlayUIBackEvent()
	{
		if (uiBack != null) PlaySFX(uiBack);
		else Debug.LogWarning("No sound for return button");
	}
	#endregion

	#region Joueur
	[Header("UI buttons sounds")]
	[SerializeField] private AudioClip chatNo;
	[SerializeField] private AudioClip chatYes;
	[SerializeField] private AudioClip bzz;
	[SerializeField] private AudioClip dirt;
	[SerializeField] private AudioClip grass;
	[SerializeField] private AudioClip concrete;
	public void PlayCatNo()
	{
		if (chatNo != null) PlaySFX(chatNo);
		else Debug.LogWarning("No sound for no cat");
	}
	public void PlayCat()
	{
		if (chatYes != null) PlaySFX(chatYes);
		else Debug.LogWarning("No sound for cat");
	}
	public void PlayBzz()
	{
		if (bzz != null) PlaySFX(bzz);
		else Debug.LogWarning("No sound for electrocution");
	}
	public void PlayDirt()
	{
		if (dirt != null) PlaySFX(dirt);
		else Debug.LogWarning("No sound for dirt");
	}
	public void PlayGrass()
	{
		if (grass != null) PlaySFX(grass);
		else Debug.LogWarning("No sound for grass");
	}
	public void PlayConcrete()
	{
		if (concrete != null) PlaySFX(concrete);
		else Debug.LogWarning("No sound for concrete");
	}
	#endregion

	public void PlayRadar(float pitch)
	{
		radarSource.pitch = pitch;
		radarSource.Play();
	}
	public void StopRadar()
	{
		radarSource.Stop();
	}
}
