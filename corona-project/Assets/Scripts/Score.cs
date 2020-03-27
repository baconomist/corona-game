using System;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private int _score = 0;
    //private int _itemScore = 0;
    
    private void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _textMeshPro.SetText(_score.ToString());
        //_score = Mathf.RoundToInt(Time.time * 100) + _itemScore;
        //_score = _itemScore;
    }

    public void AddItemScore(int amount)
    {
        _score += amount;
        
        if(_score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", _score);
    }
}