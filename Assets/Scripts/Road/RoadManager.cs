using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class RoadManager : MonoBehaviour
    {
        [Header("Objects")]
        private List<RoadPart> _roadPartsPool = new List<RoadPart>();
        private List<RoadCross> _roadCrossPool = new List<RoadCross>();
        private List<RoadPart> _roadPartsInMap = new List<RoadPart>();
        private List<RoadCross> _roadCrossInMap = new List<RoadCross>();

        [Header("Settings")]
        [SerializeField] private int _amountRoadParts;
        [SerializeField] private int _maxAmountStraightRoads;
        [SerializeField] private int _maxLinesAmount;
        [SerializeField] private float _distanceBetweenLines;

        [Header("Variables")]
        private int _currentAmountStraightRoads;
        public int CentralLine { get; private set; }
        public int MinLine { get; private set; }
        public int MaxLine { get; private set; }
        public int DirectionX { get; private set; }
        public int DirectionZ { get; private set; }
        public Vector3 RoadRotate { get; private set; }

        [Header("Scripts")]
        [SerializeField] private RoadSpawner _roadSpawnerScr;
        [SerializeField] private RoadBuilder _roadBuilderScr;
        [SerializeField] private PlayerMovement _playerMovementScr;
        [SerializeField] private WeaponManager _playerWeaponManagerScr;

        private void Awake()
        {
            CentralLine = (_maxLinesAmount - 1) / 2;

            _roadSpawnerScr.FillPools();
            _playerMovementScr.SetLine(CentralLine);
            SetFirstDirections();

            FirstBuild();
            _playerWeaponManagerScr.ChangeDirection();
        }

        //Functions for build
        public void FirstBuild()
        {
            _currentAmountStraightRoads = 0;
            for (int i = 0; i < _amountRoadParts; i++)
            {
                ChooseRoadPart();
            }

            SetMinAndMaxLine(_roadPartsInMap[0].GetLinesAmount());
        }
        public void ReplaceRoadPart(bool isCross)
        {
            if (!isCross)
            {
                RoadPart roadPart = _roadPartsInMap[0];
                roadPart.gameObject.SetActive(false);

                _roadPartsInMap.RemoveAt(0);
                _roadPartsPool.Add(roadPart);
            }
            else
            {
                RoadCross roadCross = _roadCrossInMap[0];
                roadCross.gameObject.SetActive(false);

                _roadCrossInMap.RemoveAt(0);
                _roadCrossPool.Add(roadCross);
            }

            if (_currentAmountStraightRoads < _maxAmountStraightRoads) ChooseRoadPart();
            else if (_currentAmountStraightRoads == _maxAmountStraightRoads) ChooseRoadCross();
        }
        private void ChooseRoadCross()
        {
            int number = Random.Range(0, _roadCrossPool.Count);
            RoadCross roadCross = _roadCrossPool[number];
            _currentAmountStraightRoads++;

            _roadCrossPool.RemoveAt(number);
            _roadCrossInMap.Add(roadCross);

            _roadBuilderScr.BuildRoadPart(roadCross);
        }
        private void ChooseRoadPart()
        {
            int number = Random.Range(0, _roadPartsPool.Count);
            RoadPart roadPart = _roadPartsPool[number];
            _currentAmountStraightRoads++;

            _roadPartsPool.RemoveAt(number);
            _roadPartsInMap.Add(roadPart);

            _roadBuilderScr.BuildRoadPart(roadPart);
        }
        public void SetMinAndMaxLine(int amount)
        {
            int delta = (int)Mathf.Floor(amount / 2f);

            MinLine = CentralLine - delta;
            MaxLine = CentralLine + delta;

            if (MaxLine + 1 - MinLine > amount) MaxLine--;
        }

        //Functions for Direction
        public void ChangeDirection(int turnDirection)
        {
            _roadBuilderScr.SetTurnOffset(_roadCrossInMap[0], turnDirection);

            if (DirectionX != 0)
            {
                DirectionZ = DirectionX * -turnDirection;
                DirectionX = 0;
            }
            else if (DirectionZ != 0)
            {
                DirectionX = DirectionZ * turnDirection;
                DirectionZ = 0;
            }

            RoadRotate += new Vector3(0f, turnDirection * 90f, 0f);
            if (RoadRotate.y == 360) RoadRotate = Vector3.zero;
            else if (RoadRotate.y == -360) RoadRotate = Vector3.zero;

            _playerWeaponManagerScr.ChangeDirection();
        }
        private void SetFirstDirections()
        {
            DirectionZ = 1;
            DirectionX = 0;
            RoadRotate = Vector3.zero;
        }

        public void StartSkirmish(bool isWithDelay) 
        {
            _roadPartsInMap[0].StartSkirmish(isWithDelay);
            _playerWeaponManagerScr.ChangeSkirmishTargets(_roadPartsInMap[0].GetSkirmishTarget());
        }


        //Functions for set
        public void FillRoadPartPool(RoadPart roadPart) => _roadPartsPool.Add(roadPart);
        public void FillRoadCrossPool(RoadCross roadCross) => _roadCrossPool.Add(roadCross);

        //Functions for get
        public float GetDistanceBetweenLines() => _distanceBetweenLines;
        public Vector3 GetEndTurnPosition(int line) => _roadCrossInMap[0].GetEndTurnPosition(line);
    }
}