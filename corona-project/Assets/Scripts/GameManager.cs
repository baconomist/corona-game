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


    public GameObject ground;
    public Transform infectedPlebs;
    public Transform basePlebs;

    [HideInInspector] public Player player;
    [HideInInspector] public Score score;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        score = FindObjectOfType<Score>();
    }

    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void OnApplicationQuit()
    {
        _instance = null;
    }
}