using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintBar : MonoBehaviour
{
    private float _percent = 100.0f;
    private RawImage _rawImage;

    void Start()
    {
        _rawImage = GetComponent<RawImage>();
    }

    public void SetPercent(float percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        _rawImage.transform.localScale = new Vector3(percent / 100.0f, _rawImage.transform.localScale.y,
            _rawImage.transform.localScale.z);
    }

    public void DecreaseBy(float percent)
    {
        _percent -= percent;
        _percent = Mathf.Clamp(_percent, 0, 100);
        
        SetPercent(_percent);
    }

    public float Percent
    {
        get => _percent;
    }
}