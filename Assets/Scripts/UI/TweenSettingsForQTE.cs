using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ZombiRunner
{
    public class TweenSettingsForQTE : MonoBehaviour
    {
        [Header("Appear settings")]
        [SerializeField] private float _appearDeltaPosition;
        [SerializeField] private float _appearDuration;
        [SerializeField] private Ease _appearEase;
        [SerializeField] private float _appearFadeDuration;
        [SerializeField] private Ease _appearFadeEase;

        [Header("Timer settings")]
        [SerializeField] private float _timer;
        [SerializeField] private Ease _timerEase;

        [Header("Win settings")]
        [SerializeField] private float _winDeltaPosition;
        [SerializeField] private float _winMoveDuration;
        [SerializeField] private Ease _winMoveEase;
        [SerializeField] private float _winFadeDelay;
        [SerializeField] private float _winFadeDuration;
        [SerializeField] private Ease _winFadeEase;

        [Header("Lose settings")]
        [SerializeField] private float _loseShakeDuration;
        [SerializeField] private Vector3 _loseShakeStrength;
        [SerializeField] private int _loseShakeVibrato;
        [SerializeField] private float _loseShakeRandomness;
        [SerializeField] private Ease _loseShakeEase;
        [SerializeField] private float _loseFadeDelay;
        [SerializeField] private float _loseFadeDuration;
        [SerializeField] private Ease _loseFadeEase;

        public float GetDeltaAppearPosition() => _appearDeltaPosition;
        public float GetAppearDuration() => _appearDuration;
        public Ease GetAppearEase() => _appearEase;
        public float GetAppearFadeDuration() => _appearFadeDuration;
        public Ease GetAppearFadeEase() => _appearFadeEase;

        public float GetTimer() => _timer;
        public Ease GetTimerEase() => _timerEase;

        public float GetWinDeltaPosition() => _winDeltaPosition;
        public float GetWinMoveDuration() => _winMoveDuration;
        public Ease GetWinMoveEase() => _winFadeEase;
        public float GetWinFadeDelay() => _winFadeDelay;
        public float GetWinFadeDuration() => _winFadeDuration;
        public Ease GetWinFadeEase() => _winFadeEase;

        public float GetLoseShakeDuration() => _loseShakeDuration;
        public Vector3 GetLoseShakeStrength() => _loseShakeStrength;
        public int GetLoseShakeVibrato() => _loseShakeVibrato;
        public float GetLoseShakeRandomness() => _loseShakeRandomness;
        public Ease GetLoseShakeEase() => _loseShakeEase;
        public float GetLoseFadeDelay() => _loseFadeDelay;
        public float GetLoseFadeDuration() => _loseFadeDuration;
        public Ease GetLoseFadeEase() => _loseFadeEase;

    }
}