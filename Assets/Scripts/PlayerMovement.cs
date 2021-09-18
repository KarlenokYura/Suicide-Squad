using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _bulletBorder;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private ControllerBorder _controllerBorder;
    [SerializeField]
    public GameManager _gameManager;
    [SerializeField]
    private float _playerSpeed = 1.0f;

    void Awake()
    {
        _controllerBorder = GameObject.FindGameObjectWithTag("ControllerBorder").GetComponent<ControllerBorder>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (photonView.IsMine)
        {
            _camera.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (_controllerBorder.Horizontal() != 0.0f || _controllerBorder.Vertical() != 0.0f)
            {
                Vector2 direction = new Vector2(_controllerBorder.Horizontal(), _controllerBorder.Vertical());
                transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, _playerSpeed * Time.deltaTime);
            }
        }
    }

    [PunRPC]
    public void MasterSetBulletBorder(bool isBullet)
    {
        photonView.RPC("AllSetBulletBorder", RpcTarget.AllBuffered, isBullet);
    }

    [PunRPC]
    public void AllSetBulletBorder(bool isBullet)
    {
        if (isBullet)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
            _bulletBorder.gameObject.SetActive(true);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            _bulletBorder.gameObject.SetActive(false);
        }
        _bulletBorder.GetComponent<BulletBorder>()._isFirstCollision = false;
    }
}
