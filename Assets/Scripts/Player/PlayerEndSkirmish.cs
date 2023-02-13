using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class PlayerEndSkirmish : MonoBehaviour
    {
        [Header("Scripts")]
        [SerializeField] private PlayerMovement _playerMovementScr;
        [SerializeField] private PlayerAnimations _playerAnimationsScr;
        [SerializeField] private QuickTimeEventsUI _quickTimeEventsUIScr;
        
        public IEnumerator EndSkirmish()
        {
            _quickTimeEventsUIScr.StartEvent(SwipeController.SwipeType.UP);
            while (_quickTimeEventsUIScr.IsEventWorking) { yield return null; }
            if (!_quickTimeEventsUIScr.IsLastEventWin) yield break;
            StartCoroutine(_playerMovementScr.Climb());
        }
    }
}