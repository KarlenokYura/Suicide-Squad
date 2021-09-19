using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using LootLocker.Requests;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    public int _bulletPlayerID = -1;
    [SerializeField]
    public int _bulletPriorPlayerID = -1;
    [SerializeField]
    private Image _timerBorder;
    [SerializeField]
    private Image _timerBar;
    [SerializeField]
    private Text _stopwatch;

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f)), Quaternion.identity);
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
        });
        photonView.RPC("MasterGetTimerBarTime", RpcTarget.MasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(new string[] { PhotonNetwork.NickName });
            if (_bulletPlayerID == -1 && PhotonNetwork.CurrentRoom.PlayerCount > 2)
            {
                StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
                photonView.RPC("ActivateTimerBar", RpcTarget.AllBuffered, true);
                photonView.RPC("ActivateStopwatch", RpcTarget.AllBuffered, true);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("MasterSetLeaderboard", RpcTarget.MasterClient, otherPlayer.NickName);
            if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    photonView.RPC("MasterSetLeaderboard", RpcTarget.MasterClient, player.NickName);
                }
                StartCoroutine("SetBulletToPlayer", -1);
                if (otherPlayer.ActorNumber != _bulletPlayerID)
                {
                    photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, _bulletPlayerID, false);
                }
                photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, -1, false);
                photonView.RPC("ActivateTimerBar", RpcTarget.AllBuffered, false);
                photonView.RPC("ActivateStopwatch", RpcTarget.AllBuffered, false);
            }
            else if (otherPlayer.ActorNumber == _bulletPlayerID)
            {
                StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
                photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, -1, false);
            }
        }
    }

    public IEnumerator SetBulletToPlayer(int playerId)
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("MasterSetBulletToPlayer", RpcTarget.MasterClient, playerId, true);
    }

    [PunRPC]
    public void MasterSetBulletToPlayer(int playerId, bool isBullet)
    {
        photonView.RPC("AllSetBulletToPlayer", RpcTarget.AllBuffered, playerId);
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.CreatorActorNr == playerId)
            {
                GameObject playerPrefabObject = photonView.gameObject;
                playerPrefabObject.GetPhotonView().RPC("MasterSetBulletBorder", RpcTarget.MasterClient, isBullet);
            }
        }
    }

    [PunRPC]
    public void AllSetBulletToPlayer(int playerId)
    {
        _bulletPlayerID = playerId;
    }

    [PunRPC]
    public void MasterSetBulletToPriorPlayer(int priorPlayerId, bool isBullet)
    {
        photonView.RPC("AllSetBulletToPriorPlayer", RpcTarget.AllBuffered, priorPlayerId);
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.CreatorActorNr == priorPlayerId)
            {
                GameObject playerPrefabObject = photonView.gameObject;
                playerPrefabObject.GetPhotonView().RPC("MasterSetBulletBorder", RpcTarget.MasterClient, isBullet);
            }
        }
    }

    [PunRPC]
    public void AllSetBulletToPriorPlayer(int priorPlayerId)
    {
        _bulletPriorPlayerID = priorPlayerId;
    }

    [PunRPC]
    public void MasterGetTimerBarTime()
    {
        photonView.RPC("AllSetTimerBarTime", RpcTarget.AllBuffered, _timerBar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    [PunRPC]
    public void AllSetTimerBarTime(float timerBarTime)
    {
        _timerBar.GetComponent<Animator>().Play("TimerBarAnimation", 0, timerBarTime);
    }

    [PunRPC]
    public void ActivateTimerBar(bool isActive)
    {
        if (isActive)
        {
            _timerBar.GetComponent<Animator>().Play("TimerBarAnimation", 0, 0);
        }
        _timerBorder.gameObject.SetActive(isActive);
    }

    [PunRPC]
    public void ActivateStopwatch(bool isActive)
    {
        _stopwatch.gameObject.SetActive(isActive);
    }

    public void SuicidePlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_bulletPlayerID != -1)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.ActorNumber == _bulletPlayerID)
                    {
                        if (player.IsMasterClient)
                        {
                            photonView.RPC("MasterSetLeaderboard", RpcTarget.MasterClient, player.NickName);
                        }
                        photonView.RPC("KickPlayer", player);
                    }
                }
            }
            photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, -1, false);
        }
    }

    [PunRPC]
    public void KickPlayer()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void MasterSetLeaderboard(string nickname)
    {
        int time = 0;
        if (_stopwatch.IsActive())
        {
            string[] stopwatch = _stopwatch.GetComponent<Text>().text.Split(':');
            time = System.Int32.Parse(stopwatch[3]) + (System.Int32.Parse(stopwatch[2]) * 1000) + (System.Int32.Parse(stopwatch[1]) * 1000 * 60) + (System.Int32.Parse(stopwatch[0]) * 1000 * 60 * 60);
        }
        LootLockerSDKManager.SubmitScore(nickname, time, 460, null);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
