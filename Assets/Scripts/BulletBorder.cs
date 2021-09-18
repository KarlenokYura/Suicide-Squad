using Photon.Pun;
using UnityEngine;

public class BulletBorder : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    public bool _isFirstCollision = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerPrefab.GetComponent<PhotonView>().IsMine && collision.gameObject.layer == 8 && collision.GetComponent<PhotonView>().CreatorActorNr != _playerPrefab.GetComponent<PlayerMovement>()._gameManager.GetComponent<GameManager>()._bulletPriorPlayerID && !_isFirstCollision)
        {
            _isFirstCollision = true;
            _playerPrefab.GetComponent<PlayerMovement>()._gameManager.GetComponent<GameManager>().GetComponent<PhotonView>().RPC("MasterSetBulletToPriorPlayer", RpcTarget.MasterClient, _playerPrefab.GetComponent<PhotonView>().CreatorActorNr, false);
            _playerPrefab.GetComponent<PlayerMovement>()._gameManager.GetComponent<GameManager>().GetComponent<PhotonView>().RPC("MasterSetBulletToPlayer", RpcTarget.MasterClient, collision.GetComponent<PhotonView>().CreatorActorNr, true);
        }
    }
}
