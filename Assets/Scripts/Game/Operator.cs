using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class Operator : MonoBehaviour
    {
        [SerializeField] private GameObject _realPlayer;
        [SerializeField] private GameObject player;

        [Header("Used settings")]
        [SerializeField] private Vector3 _deltaPosition;
        [SerializeField] private Vector3 _deltaRotation;
        [SerializeField] private float _minYPosition;

        [Header("Transition settings")]
        [SerializeField] private float _transitionDuration;

        [Header("First person mode settings")]
        [SerializeField] private Vector3 _deltaPositionFPM;
        [SerializeField] private Vector3 _deltaRotationFPM;
        [SerializeField] private float _minYPositionFPM;

        [Header("Third person mode 1 settings")]
        [SerializeField] private Vector3 _deltaPositionTPM1;
        [SerializeField] private Vector3 _deltaRotationTPM1;
        [SerializeField] private float _minYPositionTPM1;

        [Header("Third person mode 2 settings")]
        [SerializeField] private Vector3 _deltaPositionTPM2;
        [SerializeField] private Vector3 _deltaRotationTPM2;
        [SerializeField] private float _minYPositionTPM2;

        [Header("Melee combat mode settings")]
        [SerializeField] private Vector3 _deltaPositionMCM;
        [SerializeField] private Vector3 _deltaRotationMCM;
        [SerializeField] private float _minYPositionMCM;

        [Header("Scripts")]
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private PlayerController _playerControllerScr;

        private void Awake()
        {
            SetFPM();
        }
        private void FixedUpdate()
        {
            SetCameraMode();
        }

        private void SetCameraMode()
        {
            Vector3 newCameraPosition = new Vector3(player.transform.position.x + _deltaPosition.x * _roadManagerScr.DirectionZ + _deltaPosition.z * _roadManagerScr.DirectionX,
                                                    player.transform.position.y + _deltaPosition.y,
                                                    player.transform.position.z + _deltaPosition.z * _roadManagerScr.DirectionZ + _deltaPosition.x * -_roadManagerScr.DirectionX);

            if (newCameraPosition.y < _minYPosition) newCameraPosition.y = _minYPosition;

            transform.position = newCameraPosition;
            transform.rotation = Quaternion.Euler(_deltaRotation.x, _realPlayer.transform.rotation.eulerAngles.y + _deltaRotation.y, _deltaRotation.z);
        }
        private void SetFPM()
        {
            _deltaPosition = _deltaPositionFPM;
            _deltaRotation = _deltaRotationFPM;
            _minYPosition = _minYPositionFPM;

            _playerControllerScr.HideTPModel();
        }
        public IEnumerator TransitionToMCM()
        {
            _deltaPosition = _deltaPositionTPM1;
            _deltaRotation = _deltaRotationTPM1;
            _minYPosition = _minYPositionTPM1;
            _playerControllerScr.ShowTPModel();

            for (float t = 0; t < _transitionDuration; t += Time.fixedDeltaTime)
            {
                _deltaPosition = Vector3.Lerp(_deltaPositionTPM1, _deltaPositionMCM, t / _transitionDuration);
                _deltaRotation = Vector3.Lerp(_deltaRotationTPM1, _deltaRotationMCM, t / _transitionDuration);
                _minYPosition = Mathf.Lerp(_minYPositionTPM1, _minYPositionMCM, t / _transitionDuration);

                yield return new WaitForFixedUpdate();
            }

            _deltaPosition = _deltaPositionMCM;
            _deltaRotation = _deltaRotationMCM;
            _minYPosition = _minYPositionMCM;
        }
        public IEnumerator TransitionToTPM2()
        {
            _deltaPosition = _deltaPositionTPM1;
            _deltaRotation = _deltaRotationTPM1;
            _minYPosition = _minYPositionTPM1;
            _playerControllerScr.ShowTPModel();

            for (float t = 0; t < _transitionDuration; t += Time.fixedDeltaTime)
            {
                _deltaPosition = Vector3.Lerp(_deltaPositionTPM1, _deltaPositionTPM2, t / _transitionDuration);
                _deltaRotation = Vector3.Lerp(_deltaRotationTPM1, _deltaRotationTPM2, t / _transitionDuration);
                _minYPosition = Mathf.Lerp(_minYPositionTPM1, _minYPositionTPM2, t / _transitionDuration);

                yield return new WaitForFixedUpdate();
            }

            _deltaPosition = _deltaPositionTPM2;
            _deltaRotation = _deltaRotationTPM2;
            _minYPosition = _minYPositionTPM2;
        }
        public IEnumerator TransitionToFPM()
        {
            Vector3 deltaPosition = _deltaPosition;
            Vector3 deltaRotation = _deltaRotation;
            float minYPosition = _minYPosition;

            for (float t = 0; t < _transitionDuration; t += Time.fixedDeltaTime)
            {
                _deltaPosition = Vector3.Lerp(deltaPosition, _deltaPositionTPM1, t / _transitionDuration);
                _deltaRotation = Vector3.Lerp(deltaRotation, _deltaRotationTPM1, t / _transitionDuration);
                _minYPosition = Mathf.Lerp(minYPosition, _minYPositionTPM1, t / _transitionDuration);

                yield return new WaitForFixedUpdate();
            }

            SetFPM();
        }
    }
}