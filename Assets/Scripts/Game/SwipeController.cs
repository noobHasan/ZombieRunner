using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [Header("Variables")]
    private Vector2 _tapPoint;
    private Vector2 _swipeDelta;
    private bool _isDragging;
    private bool _isMobilePlatform;
    private float _currentClickTime;
    private bool _isSwipe;

    [Header("Swipe settings")]
    [SerializeField] private float _minSwipeDelta;
    [SerializeField] private float _clickTime;

    public enum SwipeType
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        CLICK
    }

    public delegate void OnSwipeInput(SwipeType type);
    public static event OnSwipeInput SwipeEvent;

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        _isMobilePlatform = false;
#else
        _isMobilePlatform = true;
#endif
    }

    private void Update()
    {
        _currentClickTime -= Time.fixedDeltaTime;

        if (!_isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                _tapPoint = Input.mousePosition;
                _currentClickTime = _clickTime;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
                if (_currentClickTime > 0 && !_isSwipe) SwipeEvent(SwipeType.CLICK);

                _isSwipe = false;
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if(Input.touches[0].phase == TouchPhase.Began)
                {
                    _isDragging = true;
                    _tapPoint = Input.touches[0].position;
                    _currentClickTime = _clickTime;
                }
                else if (Input.touches[0].phase == TouchPhase.Canceled ||
                         Input.touches[0].phase == TouchPhase.Ended)
                {
                    ResetSwipe();
                    if (_currentClickTime > 0 && !_isSwipe) SwipeEvent(SwipeType.CLICK);

                    _isSwipe = false;
                }
            }
        }

        CalculateSwipe();
    }

    private void CalculateSwipe()
    {
        _swipeDelta = Vector2.zero;

        if (_isDragging)
        {
            if (!_isMobilePlatform && Input.GetMouseButton(0)) _swipeDelta = (Vector2)Input.mousePosition - _tapPoint;
            else if (_isMobilePlatform && Input.touchCount > 0) _swipeDelta = Input.touches[0].position - _tapPoint;
        }

        if (_swipeDelta.magnitude > _minSwipeDelta)
        {
            if (Mathf.Abs(_swipeDelta.x) < Mathf.Abs(_swipeDelta.y) && _swipeDelta.y > 0) SwipeEvent(SwipeType.UP);
            else if (Mathf.Abs(_swipeDelta.x) < Mathf.Abs(_swipeDelta.y) && _swipeDelta.y < 0) SwipeEvent(SwipeType.DOWN);
            else if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y) && _swipeDelta.x > 0) SwipeEvent(SwipeType.RIGHT);
            else if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y) && _swipeDelta.x < 0) SwipeEvent(SwipeType.LEFT);

            _isSwipe = true;
            ResetSwipe();
        }
    }

    public void ResetSwipe()
    {
        _isDragging = false;
        _tapPoint = _swipeDelta - Vector2.zero;
    }
}
