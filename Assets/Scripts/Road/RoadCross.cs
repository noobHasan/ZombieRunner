using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class RoadCross : RoadPart
    {
        private Dictionary<int, GameObject> _turningLine = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject[] _leftEndTurn;
        [SerializeField] private GameObject[] _forwardEndTurn;
        [SerializeField] private GameObject[] _rightEndTurn;

        private void Start()
        {
            FillTurningLine();
        }

        private void FillTurningLine()
        {
            int line = 0;
            for (int i = 0; i < _leftEndTurn.Length; i++)
            {
                _turningLine.Add(line, _leftEndTurn[i]);
                line++;
            }
            for (int i = 0; i < _forwardEndTurn.Length; i++)
            {
                _turningLine.Add(line, _forwardEndTurn[i]);
                line++;
            }
            for (int i = 0; i < _rightEndTurn.Length; i++)
            {
                _turningLine.Add(line, _rightEndTurn[i]);
                line++;
            }
        }

        public Vector3 GetEndTurnPosition(int number) => _turningLine[number].transform.position;
    }
}