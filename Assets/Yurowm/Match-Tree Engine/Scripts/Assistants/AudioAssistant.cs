using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioListener))]
[RequireComponent (typeof (AudioSource))]
public class AudioAssistant : MonoBehaviour {

	public static AudioAssistant main;

	AudioSource music;
	AudioSource sfx;

	public float musicVolume = 1f;

	public AudioClip menuMusic;
	public AudioClip fieldMusic;
	
	public AudioClip[] chipHit;
	public AudioClip[] chipCrush;
	public AudioClip[] swapSuccess;
	public AudioClip[] swapFailed;
	public AudioClip[] bombCrush;
	public AudioClip[] crossBombCrush;
	public AudioClip[] colorBombCrush;
	public AudioClip[] createBomb;
	public AudioClip[] createCrossBomb;
	public AudioClip[] createColorBomb;
	public AudioClip[] timeWarrning;
	public AudioClip[] youWin;
	public AudioClip[] youLose;
	public AudioClip[] buy;
	
	static Dictionary<string, AudioClip[]> data = new Dictionary<string, AudioClip[]>();
	static List<string> mixBuffer = new List<string>();
	static float mixBufferClearDelay = 0.05f;

	void Awake () {
		main = this;

		AudioSource[] sources = GetComponents<AudioSource> ();
		music = sources[0];
		sfx = sources[1];

		data.Clear ();
		data.Add ("ChipHit", chipHit);
		data.Add ("ChipCrush", chipCrush);
		data.Add ("BombCrush", bombCrush);
		data.Add ("CrossBombCrush", crossBombCrush);
		data.Add ("ColorBombCrush", colorBombCrush);
		data.Add ("SwapSuccess", swapSuccess);
		data.Add ("SwapFailed", swapFailed);
		data.Add ("CreateBomb", createBomb);
		data.Add ("CreateCrossBomb", createCrossBomb);
		data.Add ("CreateColorBomb", createColorBomb);
		data.Add ("TimeWarrning", timeWarrning);
		data.Add ("YouWin", youWin);
		data.Add ("YouLose", youLose);
		data.Add ("Buy", buy);
		StartCoroutine (MixBufferRoutine ());
	}

	IEnumerator MixBufferRoutine() {
		while (true) {
			yield return new WaitForSeconds(mixBufferClearDelay);
			mixBuffer.Clear();
		}
	}

	public void PlayMusic(string track) {
		AudioClip to = null;
		switch (track) {
		case "Menu": to = main.menuMusic; break;
		case "Field": to = main.fieldMusic; break;
		}
		if (to) 
			StartCoroutine(main.CrossFade(to));
	}

	IEnumerator CrossFade(AudioClip to) {
		float delay = 1f;
		if (music.clip != null) {
			while (delay > 0) {
				music.volume = delay * musicVolume;
				delay -= Time.unscaledDeltaTime;
				yield return 0;
			}
		}
		music.clip = to;
		if (!music.isPlaying) music.Play();
		while (delay < 0) {
			music.volume = delay * musicVolume;
			delay += Time.unscaledDeltaTime;
			yield return 0;
		}
		music.volume = musicVolume;
	}

	public static void Shot(string clip) {
		if (data.ContainsKey (clip) && !mixBuffer.Contains(clip)) {
			if (data[clip].Length == 0) return;
			mixBuffer.Add(clip);
			main.sfx.PlayOneShot(data[clip][Random.Range(0, data[clip].Length)]);
		}
	}
}
