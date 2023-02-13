using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _gameFPS;

        private void Awake()
        {
            Application.targetFrameRate = _gameFPS;
        }
    }
}