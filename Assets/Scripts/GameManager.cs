using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    public int _bulletPlayerID = -1;
    [SerializeField]
    public int _bulletPriorPlayerID = -1;
    [SerializeField]
    private IEnumerator _coroutine;
    [SerializeField]
    private bool _coroutineIsRunning;

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f)), Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_bulletPlayerID == -1 && PhotonNetwork.CurrentRoom.PlayerCount > 2)
            {
                if (!_coroutineIsRunning)
                {
                    _coroutine = SetBulletToPlayer(PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
                    StartCoroutine(_coroutine);
                }
                //StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
            {
                if (!_coroutineIsRunning)
                {
                    _coroutine = SetBulletToPlayer(-1);
                    StartCoroutine(_coroutine);
                }
                if (otherPlayer.ActorNumber != _bulletPlayerID)
                {
                    photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, _bulletPlayerID, false);
                }
                photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, -1, false);
            }
            else if (otherPlayer.ActorNumber == _bulletPlayerID)
            {
                if (!_coroutineIsRunning)
                {
                    _coroutine = SetBulletToPlayer(PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
                    StartCoroutine(_coroutine);
                }
                //StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
                photonView.RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, -1, false);
            }
        }
    }

    public IEnumerator SetBulletToPlayer(int playerId)
    {
        _coroutineIsRunning = true;
        yield return new WaitForSeconds(0.25f);
        photonView.RPC("MasterSetBulletToPlayer", RpcTarget.MasterClient, playerId, true);
        _coroutineIsRunning = false;
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
}
