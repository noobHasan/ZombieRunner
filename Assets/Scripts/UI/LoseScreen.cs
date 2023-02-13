using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombiRunner
{
    public class LoseScreen : MonoBehaviour
    {
        [Header("Scripts")]
        [SerializeField] private SwipeController _swipeControllerScr;

        public void ReatartGame()
        {
            _swipeControllerScr.ResetSwipe();
            SceneManager.LoadScene(0);
        }
    }
}