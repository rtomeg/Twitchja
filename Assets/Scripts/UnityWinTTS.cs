using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using SpeechLib;
using UnityEngine.PlayerLoop;
using Random = System.Random;

public class UnityWinTTS : MonoBehaviour
{
	private SpVoice voice;
	private void Start()
	{
		voice = new SpVoiceClass();

		StartCoroutine(VoiceModule());

		foreach (ISpeechObjectToken VARIABLE in voice.GetVoices())
		{
			Debug.Log(VARIABLE.Id);
		}

		voice.Voice = voice.GetVoices().Item(1);
		Debug.Log(voice.GetVoices().Count);
		EventsManager.onCommandReceived += Speak;
		/*
		Speak("yo le pondria el truncar a 25 carácteres antes del quitarle los carácteres no permitidos, para que trabaje menos al chequear y quitar"); // Some input object to pass to the thread.
		yield return new WaitForSeconds(1);
		Speak("yo le pondria el truncar a 25 carácteres antes del quitarle los carácteres no permitidos, para que trabaje menos al chequear y quitar"); // Some input object to pass to the thread.
	*/
	}

	private void OnDestroy()
	{
		EventsManager.onCommandReceived -= Speak;
	}

	private IEnumerator VoiceModule ()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
		voice.Rate = UnityEngine.Random.Range(-10, 5);
		StartCoroutine(VoiceModule());
	}

	public void Speak (string user, string message) {
		/*
		if (voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
		{
			voice.Pause();
		}
		voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync);
		*/
		voice.Voice = voice.GetVoices().Item(UnityEngine.Random.Range(0, voice.GetVoices().Count));
		voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
	}
}
