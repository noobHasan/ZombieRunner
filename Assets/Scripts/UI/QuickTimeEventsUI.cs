using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ZombiRunner
{
    public class QuickTimeEventsUI : MonoBehaviour
    {
        [Header("Objects for event")]
        [SerializeField] private Image _eventArrow;
        [SerializeField] private Image _eventArrowTimer;
        [SerializeField] private Image _eventArrowResult;

        [Header("Settings for event")]
        [SerializeField] private float _timeDilation;
        [SerializeField] private float _anglePosition;
        [SerializeField] private Color _winEventColor;
        [SerializeField] private Color _loseEventColor;

        [Header("Variables for event")]
        private Dictionary<SwipeController.SwipeType, Vector3> _eventDirections = new Dictionary<SwipeController.SwipeType, Vector3>();
        private Dictionary<SwipeController.SwipeType, Vector3> _eventRotations = new Dictionary<SwipeController.SwipeType, Vector3>();
        private SwipeController.SwipeType _currentEvent;
        private Coroutine _swipeCoroutine;
        public bool IsEventWorking { get; private set; }
        public bool IsLastEventWin { get; private set; }

        [Header("Scripts")]
        [SerializeField] private TweenSettingsForQTE _tweenSettingsForQTE;
        [SerializeField] private TimeLord _timeLordScr;

        private void Awake()
        {
            SetUpDictionaryDeltaPosition();
            SetUpDictionaryDirections();
            
        }

        public void StartEvent(SwipeController.SwipeType direction) => _swipeCoroutine = StartCoroutine(Event(direction));
        private IEnumerator Event(SwipeController.SwipeType direction)
        {
            IsEventWorking = true;
            IsLastEventWin = false;
            _currentEvent = direction;

            _timeLordScr.ChangeTimeSpeed(_timeDilation);

            _eventArrow.transform.localRotation = Quaternion.Euler(_eventRotations[direction]);
            _eventArrow.transform.localPosition = _eventDirections[direction] * _tweenSettingsForQTE.GetDeltaAppearPosition();
            _eventArrow.gameObject.SetActive(true);

            _eventArrowTimer.fillAmount = 1;

            _eventArrow.color = new Color(_eventArrow.color.r, _eventArrow.color.g, _eventArrow.color.b, 0);
            _eventArrow.DOFade(1, _tweenSettingsForQTE.GetAppearFadeDuration()).SetEase(_tweenSettingsForQTE.GetAppearFadeEase());

            _eventArrowTimer.color = new Color(_eventArrowTimer.color.r, _eventArrowTimer.color.g, _eventArrowTimer.color.b, 0);
            _eventArrowTimer.DOFade(1, _tweenSettingsForQTE.GetAppearFadeDuration()).SetEase(_tweenSettingsForQTE.GetAppearFadeEase());

            Vector3 midPosition = new Vector3(0f, 0f, 0f);
            _eventArrow.transform.DOLocalMove(midPosition, _tweenSettingsForQTE.GetAppearDuration()).SetEase(_tweenSettingsForQTE.GetAppearEase());
            yield return new WaitForSeconds(_tweenSettingsForQTE.GetAppearDuration());

            _eventArrowTimer.DOFillAmount(0, _tweenSettingsForQTE.GetTimer()).SetEase(_tweenSettingsForQTE.GetTimerEase());
            yield return new WaitForSeconds(_tweenSettingsForQTE.GetTimer());

            StartCoroutine(EventResult(false));
        }
        public void CheckEvent(SwipeController.SwipeType swipeType)
        {
            if (swipeType == _currentEvent) StartCoroutine(EventResult(true));
            else StartCoroutine(EventResult(false));

            _eventArrowTimer.DOKill();
            _eventArrowTimer.fillAmount = 0;
            StopCoroutine(_swipeCoroutine);
        }
        private IEnumerator EventResult(bool isWin)
        {
            _eventArrowResult.gameObject.SetActive(true);

            if (isWin) IsLastEventWin = true;
            else IsLastEventWin = false;
            _timeLordScr.ChangeTimeSpeed();
            IsEventWorking = false;

            if (isWin)
            {
                _eventArrowResult.color = _winEventColor;
                Vector3 newPosition = _eventArrow.transform.localPosition - _eventDirections[_currentEvent] * _tweenSettingsForQTE.GetWinDeltaPosition();
                _eventArrow.transform.DOLocalMove(newPosition, _tweenSettingsForQTE.GetWinMoveDuration()).SetEase(_tweenSettingsForQTE.GetWinMoveEase());

                yield return new WaitForSeconds(_tweenSettingsForQTE.GetWinFadeDelay());

                _eventArrow.DOFade(0, _tweenSettingsForQTE.GetWinFadeDuration()).SetEase(_tweenSettingsForQTE.GetWinFadeEase());
                _eventArrowResult.DOFade(0, _tweenSettingsForQTE.GetWinFadeDuration()).SetEase(_tweenSettingsForQTE.GetWinFadeEase());

                yield return new WaitForSeconds(_tweenSettingsForQTE.GetWinFadeDuration());
                _eventArrow.gameObject.SetActive(false);
            }
            else 
            {
                _eventArrowResult.color = _loseEventColor;
                _eventArrow.transform.DOShakePosition(_tweenSettingsForQTE.GetLoseShakeDuration(), _tweenSettingsForQTE.GetLoseShakeStrength(), _tweenSettingsForQTE.GetLoseShakeVibrato(),
                                      _tweenSettingsForQTE.GetLoseShakeRandomness()).SetEase(_tweenSettingsForQTE.GetLoseShakeEase());

                yield return new WaitForSeconds(_tweenSettingsForQTE.GetLoseFadeDelay());

                _eventArrow.DOFade(0, _tweenSettingsForQTE.GetLoseFadeDuration()).SetEase(_tweenSettingsForQTE.GetLoseFadeEase());
                _eventArrowResult.DOFade(0, _tweenSettingsForQTE.GetLoseFadeDuration()).SetEase(_tweenSettingsForQTE.GetLoseFadeEase());

                yield return new WaitForSeconds(_tweenSettingsForQTE.GetLoseFadeDuration());
                _eventArrow.gameObject.SetActive(false);
            }
        }

        private void SetUpDictionaryDeltaPosition()
        {
            _eventDirections.Add(SwipeController.SwipeType.UP, new Vector3(0f, -1, 0f));
            _eventDirections.Add(SwipeController.SwipeType.LEFT, new Vector3(1, 0f, 0f));
            _eventDirections.Add(SwipeController.SwipeType.DOWN, new Vector3(0f, 1, 0f));
            _eventDirections.Add(SwipeController.SwipeType.RIGHT, new Vector3(-1, 0f, 0f));
        }
        private void SetUpDictionaryDirections()
        {
            _eventRotations.Add(SwipeController.SwipeType.UP, new Vector3(0f, 0f, 0f));
            _eventRotations.Add(SwipeController.SwipeType.LEFT, new Vector3(0f, 0f, 90f));
            _eventRotations.Add(SwipeController.SwipeType.DOWN, new Vector3(0f, 0f, 180f));
            _eventRotations.Add(SwipeController.SwipeType.RIGHT, new Vector3(0f, 0f, 270f));
        }
    }
}