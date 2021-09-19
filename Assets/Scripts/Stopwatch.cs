using System;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private float _stopwatchTime;


    public void OnEnable()
    {
        _stopwatchTime = 0.0f;
    }

    public void Update()
    {
        _stopwatchTime += Time.deltaTime;
        TimeSpan timespan = TimeSpan.FromSeconds(_stopwatchTime);
        GetComponent<Text>().text = timespan.ToString(@"hh\:mm\:ss\:fff");
    }

    public void OnDisable()
    {
        _stopwatchTime = 0.0f;
    }
}
