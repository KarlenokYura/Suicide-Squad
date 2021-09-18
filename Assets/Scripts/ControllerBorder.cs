using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerBorder : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Image _controllerBorder;
    [SerializeField]
    private Image _controller;
    [SerializeField]
    private Vector2 _directionVector;
    
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        _directionVector = Vector2.zero;
        _controller.rectTransform.anchoredPosition = Vector2.zero;
    }
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_controllerBorder.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _controllerBorder.rectTransform.sizeDelta.x);
            pos.y = (pos.y / _controllerBorder.rectTransform.sizeDelta.x);

            _directionVector = new Vector2(pos.x * 2 - 1, pos.y * 2 - 1);
            _directionVector = (_directionVector.magnitude > 1.0f) ? _directionVector.normalized : _directionVector;

            _controller.rectTransform.anchoredPosition = new Vector2(_directionVector.x * (_controllerBorder.rectTransform.sizeDelta.x / 4), _directionVector.y * (_controllerBorder.rectTransform.sizeDelta.y / 4));
        }
    }

    public float Horizontal()
    {
        if (_directionVector.x != 0)
            return _directionVector.x;
        else
            return Input.GetAxis("Horizontal");
    }
    public float Vertical()
    {
        if (_directionVector.y != 0)
            return _directionVector.y;
        else
            return Input.GetAxis("Vertical");
    }

    void Start()
    {
        _controllerBorder = GetComponent<Image>();
        _controller = transform.GetChild(0).GetComponent<Image>();
    }

}
