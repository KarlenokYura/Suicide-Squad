using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LootLocker.Requests;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName = "AndreyLoh";
    [SerializeField]
    private Text _nicknameField;
    [SerializeField]
    private GameObject _leaderboard;

    public void Start()
    {
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
        });
    }

    public void CreateOrJoinRoom()
    {
        if (_nicknameField.GetComponent<Text>().text != null && _nicknameField.GetComponent<Text>().text != string.Empty
            && _nicknameField.GetComponent<Text>().text.Length >= 3 && _nicknameField.GetComponent<Text>().text.Length <= 20)
        {
            PhotonNetwork.NickName = _nicknameField.GetComponent<Text>().text;
            PhotonNetwork.JoinOrCreateRoom(_roomName, new Photon.Realtime.RoomOptions { CleanupCacheOnLeave = true }, null);
        }
    }

    public void LeaderboardShow()
    {
        LootLockerSDKManager.GetScoreList(460, 7, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;
                for (int i = 0; i < scores.Length; i++)
                {
                    _leaderboard.GetComponent<Leaderboard>()._timeBoard[i].text = $"{(scores[i].score / (1000 * 60 * 60))}:{(scores[i].score / (1000 * 60)) % 60}:{(scores[i].score / 1000) % 60}:{scores[i].score % 1000}";
                    _leaderboard.GetComponent<Leaderboard>()._nicknameBoard[i].text = scores[i].member_id.PadLeft(15, ' ').Substring(0, 15);
                }
                _leaderboard.SetActive(true);
            }
        });
    }

    public void LeaderboardHide()
    {
        _leaderboard.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("GameScene");
    }
}
