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
    private int _bulletPriorPlayerID = -1;
    [SerializeField]
    private int _bulletPlayerID = -1;

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f)), Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (_bulletPlayerID == -1 && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
        {
            StartCoroutine("SetBulletToPlayer", -1);
            SetBulletToPriorPlayer(-1);
        }
        else if (otherPlayer.ActorNumber == _bulletPlayerID)
        {
            StartCoroutine("SetBulletToPlayer", PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)].ActorNumber);
            SetBulletToPriorPlayer(-1);
        }
    }

    public IEnumerator SetBulletToPlayer(int playerId)
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPCSetBulletToPlayer", RpcTarget.AllBuffered, playerId);
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.CreatorActorNr == playerId)
            {
                GameObject playerPrefabObject = photonView.gameObject;
            }
        }
    }

    [PunRPC]
    public void RPCSetBulletToPlayer(int playerId)
    {
        _bulletPlayerID = playerId;
    }

    public void SetBulletToPriorPlayer(int playerId)
    {
        _bulletPriorPlayerID = playerId;
    }

    public IEnumerable Waiting(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
