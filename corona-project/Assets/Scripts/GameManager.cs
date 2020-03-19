using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public Player player;
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = FindObjectOfType<Player>();
    }

    public void Restart()
    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}