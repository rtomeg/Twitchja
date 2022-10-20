using System.Collections;
using UnityEngine;
using SpeechLib;

public class UnityWinTTS : MonoBehaviour
{
	/*
	private SpVoice voice;
	private void Start()
	{
		voice = new SpVoiceClass();

		StartCoroutine(VoiceModule());

		foreach (ISpeechObjectToken voices in voice.GetVoices())
		{
			Debug.Log(voices.Id);
		}

		voice.Voice = voice.GetVoices().Item(1);
		Debug.Log(voice.GetVoices().Count);
		EventsManager.onCommandReceived += Speak;
	}

	private void OnDestroy()
	{
		EventsManager.onCommandReceived -= Speak;
	}

	private IEnumerator VoiceModule ()
	{
		yield return new WaitForSeconds(Random.Range(0.1f, 1f));
		voice.Rate = Random.Range(-10, 5);
		StartCoroutine(VoiceModule());
	}

	public void Speak (string user, string message) {

		voice.Voice = voice.GetVoices().Item(Random.Range(0, voice.GetVoices().Count));
		voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
	}
	*/
}