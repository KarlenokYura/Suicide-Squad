using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    public void SuicidePlayer()
    {
        _gameManager.GetComponent<GameManager>().SuicidePlayer();
    }
}
