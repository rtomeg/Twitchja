using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitchData : MonoBehaviour
{
    private static TwitchData instance = null;

    public string username;
    public string channelName;
    public string password;

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField channelNameField;
    [SerializeField] private TMP_InputField passwordField;


    // Game Instance Singleton
    public static TwitchData Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void StartGame()
    {
        username = usernameField.text;
        channelName = channelNameField.text;
        password = passwordField.text;
        SceneManager.LoadScene("Ouija");

    }

    public void OpenLink()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }
}