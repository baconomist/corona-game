using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if(_instance == null) _instance = FindObjectOfType<GameManager>();
            
            // If it is still null, create a new instance
            if (_instance == null)
            {
                var obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    public bool running = false;

    public GameObject ground;
    public Transform infectedPlebs;
    public Transform basePlebs;
    public GameObject gameUI;
    public GameObject deathUI;
    public Score score;

    public AudioClip costCovidAudio;
    public AudioClip costCovidAudioIntro;

    [HideInInspector] public Player player;
    [HideInInspector] public AdManager adManager;

    private AudioSource _audioSource;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        adManager = FindObjectOfType<AdManager>();

        _audioSource = GetComponent<AudioSource>();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (running && _audioSource.clip != costCovidAudio)
            _audioSource.clip = costCovidAudio;
        
        if(!_audioSource.isPlaying)
            _audioSource.Play();
    }

    void OnApplicationQuit()
    {
        _instance = null;
    }
}