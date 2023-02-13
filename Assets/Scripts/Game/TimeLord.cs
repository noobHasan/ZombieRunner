using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class TimeLord : MonoBehaviour
    {
        public float RealTime { get; private set; }

        [Header("Change time settings")]
        [SerializeField] private float _duration;

        [Header("audio sources")]
        [SerializeField] private AudioSource[] _sources;

        [Header("Scripts")]
        [SerializeField] private PlayerAnimations _playerAnimationsScr;

        private void Awake()
        {
            InitializeTime();
        }
        public void InitializeTime() => RealTime = 1;
        public void ChangeTimeSpeed(float value = 1) => StartCoroutine(SetSpeedTime(value));
        private IEnumerator SetSpeedTime(float newValue)
        {
            float oldValue = Time.timeScale;
            for (float t = 0; t < _duration; t += Time.fixedDeltaTime)
            {
                RealTime = Mathf.Lerp(oldValue, newValue, t / _duration);
                _playerAnimationsScr.SetTimeSpeed(RealTime);

                for (int i = 0; i < _sources.Length; i++)
                {
                    _sources[i].pitch = RealTime;
                }

                yield return null;
            }
            RealTime = newValue;
        }
    }
}