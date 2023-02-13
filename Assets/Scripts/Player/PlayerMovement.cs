using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _moveDirection;
        //public int currentLineToMove;

        [Header("Objects for moving")]
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private BoxCollider _playerCollider;
        [SerializeField] private AudioSource _moveAudioSource;

        [Header("Settings for moving")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _sideDashSpeed;

        [Header("Tags")]
        [SerializeField] private string _enemyTag;
        [SerializeField] private string _turnTag;
        [SerializeField] private string _startRoadPartTag;
        [SerializeField] private string _endRoadPartTag;
        [SerializeField] private string _endRoadCrossTag;
        [SerializeField] private string _meleeCombatMode;
        [SerializeField] private string _skirmishMode;
        [SerializeField] private string _endSkirmish;
        [SerializeField] private string _coverMode;
        [SerializeField] private string _killAreaTag;
        [SerializeField] private string _stopControl;
        [SerializeField] private string _continueControl;
        [SerializeField] private string _obstaclesTag;
        [SerializeField] private string _stopShooting;

        [Header("Sounds")]
        [SerializeField] private AudioClip _runSound;
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _tackleSound;

        [Header("Variables for moving")]
        private Vector3 _normale;
        private bool _isTurning;
        private bool _isStopMove;
        private bool _isDashing;
        private Vector3 _linePosition;
        public float RunningLine { get; private set; }

        [Header("Settings for jump")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private ForceMode _jumpMode;
        [SerializeField] private LayerMask _roadMask;
        [SerializeField] private float _distanceToRoad;
        [SerializeField] private Transform _roadChecker;

        [Header("Settings for tackle")]
        [SerializeField] private Vector3 _classicColliderCenter;
        [SerializeField] private Vector3 _classicColliderSize;
        [SerializeField] private Vector3 _tackleColliderCenter;
        [SerializeField] private Vector3 _tackleColliderSize;
        [SerializeField] private Vector3 _coverColliderCenter;
        [SerializeField] private Vector3 _coverColliderSize;

        [Header("Settings for collision")]
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Vector3 _checkCollisionOffset;
        [SerializeField] private float _checkCollisionDistance;

        [Header("Scripts")]
        [SerializeField] private PlayerAnimations _playerAnimationsScr;
        [SerializeField] private PlayerController _playerControllerScr;
        [SerializeField] private PlayerMeleeCombat _playerMeleeCombatScr;
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private Health _healthScr;
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private TimeLord _timeLordScr;
        [SerializeField] private Operator _operatorScr;

        private void Awake()
        {
            MoveSetUp();
        }

        private void FixedUpdate()
        {
            PlayerMoving();
        }

        //Functions for set
        public void SetLine(int startLineNumber)
        {
            RunningLine = startLineNumber;
        }

        //Functions for move
        private void PlayerMoving()
        {
            if (_isTurning || _isStopMove || _playerControllerScr.IsDie) return;

            Vector3 directionAlongSurface = CalculateNormale(_moveDirection.normalized);
            Vector3 offset = Vector3.zero;

            offset = new Vector3(directionAlongSurface.z * _moveSpeed * (Time.fixedDeltaTime * _timeLordScr.RealTime) * _roadManagerScr.DirectionX,
                                 0f,
                                 directionAlongSurface.z * _moveSpeed * (Time.fixedDeltaTime * _timeLordScr.RealTime) * _roadManagerScr.DirectionZ);
            //offset = new Vector3(directionAlongSurface.x * _moveSpeed * Time.fixedDeltaTime, 0f, 0f);

            _playerRigidbody.MovePosition(_playerRigidbody.position + offset);
        }
        private Vector3 CalculateNormale(Vector3 forward) => forward - Vector3.Dot(forward, _normale) * _normale;

        private IEnumerator Turning(int turnDirection)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _roadManagerScr.GetEndTurnPosition((int)RunningLine);
            endPosition = new Vector3(endPosition.x, startPosition.y, endPosition.z);

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(0f, 90f * turnDirection, 0f));

            float turnDuration = 1;


            for (float t = 0; t < turnDuration; t += (Time.fixedDeltaTime * _timeLordScr.RealTime))
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, t / turnDuration);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / turnDuration);

                yield return new WaitForFixedUpdate();
            }

            transform.position = endPosition;
            transform.rotation = endRotation;

            _isTurning = false;
        }
        public IEnumerator Climb()
        {
            Jump(false);
            yield return new WaitForSeconds(0.2f);
            _playerAnimationsScr.Climb();
            yield return new WaitForSeconds(0.2f);

            float duration = 8 / _moveSpeed;
            Vector3 newPosition = Vector3.zero;
            Vector3 newNewPosition = transform.position;
            if (_roadManagerScr.DirectionZ != 0) newNewPosition.z += 2 * _roadManagerScr.DirectionZ;
            else newNewPosition.x += 2 * _roadManagerScr.DirectionX;

            for (float t = 0; t < duration; t += (Time.fixedDeltaTime * _timeLordScr.RealTime))
            {
                newPosition = transform.position;
                if (_roadManagerScr.DirectionZ != 0) newPosition.z = Mathf.Lerp(newPosition.z, newNewPosition.z, t / duration);
                else newPosition.x = Mathf.Lerp(newPosition.x, newNewPosition.x, t / duration);
                transform.position = newPosition;
                yield return null;
            }
            newPosition = transform.position;
            if (_roadManagerScr.DirectionZ != 0) newPosition.z = newNewPosition.z;
            else newPosition.x = newNewPosition.x;
            transform.position = newPosition;

            yield return null;

            ContinueMove();
        }

        //Functions for jump
        public void Jump(bool isJump)
        {
            if (Physics.Raycast(_roadChecker.position, Vector3.down, _distanceToRoad, _roadMask))
            {
                _moveAudioSource.clip = _jumpSound;
                _moveAudioSource.Play();
                if (_weaponManagerScr.IsOwnWeapon) _healthScr.TurnOnImmortality();

                _playerRigidbody.AddForce(new Vector3(0f, _jumpForce, 0f), _jumpMode);
                if (isJump) _playerAnimationsScr.StartAnimation(PlayerAnimations.State.Jump);

                StartCoroutine(LandingAfterJump());
            }
        }
        private IEnumerator LandingAfterJump()
        {
            yield return new WaitForSeconds(1);

            _moveAudioSource.clip = _runSound;
            _moveAudioSource.Play();
            if (_weaponManagerScr.IsOwnWeapon) _healthScr.TurnOffImmortality();
        }

        //Functions for tackle
        public void Tackle()
        {
            if (_weaponManagerScr.IsOwnWeapon) _healthScr.TurnOnImmortality();

            _moveAudioSource.clip = _tackleSound;
            _moveAudioSource.Play();
            _playerAnimationsScr.StartAnimation(PlayerAnimations.State.Tackle);
            _playerCollider.center = _tackleColliderCenter;
            _playerCollider.size = _tackleColliderSize;

            StartCoroutine(GetUpAfterTackle());
        }
        private IEnumerator GetUpAfterTackle()
        {
            yield return new WaitForSeconds(1);

            if (!_isStopMove) SetToNormalCollider();

            _moveAudioSource.clip = _runSound;
            _moveAudioSource.Play();
            _healthScr.TurnOffImmortality();
        }
        public IEnumerator SetSkirmishCollider()
        {
            if (!_weaponManagerScr.IsSkirmish) yield break;
            SetToNormalCollider();
            yield return new WaitForSeconds(1);
            SetCoverCollider();
        }
        public void SetCoverCollider()
        {
            _playerCollider.center = _coverColliderCenter;
            _playerCollider.size = _coverColliderSize;
        }
        public void SetToNormalCollider()
        {
            _playerCollider.center = _classicColliderCenter;
            _playerCollider.size = _classicColliderSize;
        }

        //Functions for change line
        public void StartDash(int direction)
        {
            if (RunningLine + direction < _roadManagerScr.MinLine || RunningLine + direction > _roadManagerScr.MaxLine || _isDashing) return;
            _isDashing = true;
            RunningLine += direction;

            float nextLinePosition = direction * _roadManagerScr.GetDistanceBetweenLines();
            _linePosition = transform.position + new Vector3(nextLinePosition * _roadManagerScr.DirectionZ, 0f, nextLinePosition * -_roadManagerScr.DirectionX);

            StartCoroutine(Dash());
            _playerAnimationsScr.StartDash(direction);
        }
        private IEnumerator Dash()
        {
            if (_weaponManagerScr.IsOwnWeapon) _healthScr.TurnOnImmortality();

            float duration = 10 / _sideDashSpeed;
            StartCoroutine(EndDashCoroutine(duration));

            Vector3 newPosition = Vector3.zero;
            for (float t = 0; t < duration; t += (Time.fixedDeltaTime * _timeLordScr.RealTime))
            {
                newPosition = transform.position;
                if (_roadManagerScr.DirectionZ != 0) newPosition.x = Mathf.Lerp(newPosition.x, _linePosition.x, t / duration);
                else newPosition.z = Mathf.Lerp(newPosition.z, _linePosition.z, t / duration);
                transform.position = newPosition;
                yield return null;
            }
            newPosition = transform.position;
            if (_roadManagerScr.DirectionZ != 0) newPosition.x = _linePosition.x;
            else newPosition.z = _linePosition.z;
            transform.position = newPosition;

            _isDashing = false;
            if (_weaponManagerScr.IsOwnWeapon) _healthScr.TurnOffImmortality();
        }
        private IEnumerator EndDashCoroutine(float duratiom)
        {
            yield return new WaitForSeconds(duratiom * 1f);
            _playerAnimationsScr.EndDash();
        }
        private void CalculateLinesAmount()
        {
            RaycastHit roadHit;
            Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out roadHit, 10, _roadMask);

            RoadPart roadPartScr = roadHit.collider.gameObject.GetComponentInParent<RoadPart>();
            RoadCross roadCrossScr = null;
            if (roadPartScr == null) roadCrossScr = roadHit.collider.gameObject.GetComponentInParent<RoadCross>();

            int amount = 0;
            if (roadPartScr != null)
            {
                amount = roadPartScr.GetLinesAmount();
                roadPartScr.RiseEnemies();
            }
            else 
            {
                roadCrossScr.RiseEnemies();
                amount = roadCrossScr.GetLinesAmount();
            }

            _roadManagerScr.SetMinAndMaxLine(amount);
        }

        //Functions for check obstacle
        private bool CheckObstacleInFront()
        {
            return Physics.Raycast(transform.position + _checkCollisionOffset, new Vector3(1 * _roadManagerScr.DirectionX, 0, 1 * _roadManagerScr.DirectionZ), _checkCollisionDistance, _obstacleMask);
        }
        private bool CheckObstacleInTheSide()
        {
            return (Physics.Raycast(transform.position + _checkCollisionOffset, new Vector3(1 * _roadManagerScr.DirectionZ, 0, 1 * -_roadManagerScr.DirectionX), _checkCollisionDistance, _obstacleMask)
                 || Physics.Raycast(transform.position + _checkCollisionOffset, new Vector3(-1 * _roadManagerScr.DirectionZ, 0, -1 * -_roadManagerScr.DirectionX), _checkCollisionDistance, _obstacleMask)
                 || Physics.Raycast(transform.position + _checkCollisionOffset, new Vector3(1 * _roadManagerScr.DirectionZ + 1 * _roadManagerScr.DirectionX, 0, 1 * _roadManagerScr.DirectionZ + 1 * _roadManagerScr.DirectionX), _checkCollisionDistance * 0.8f, _obstacleMask)
                 || Physics.Raycast(transform.position + _checkCollisionOffset, new Vector3(-1 * _roadManagerScr.DirectionZ + -1 * -_roadManagerScr.DirectionX, 0, 1 * _roadManagerScr.DirectionZ + 1 * -_roadManagerScr.DirectionX), _checkCollisionDistance * 0.8f, _obstacleMask));
        }

        private void MoveSetUp()
        {
            _moveDirection = Vector3.zero;
            _moveDirection.z = _moveSpeed;
        }
        private void OnCollisionEnter(Collision collision)
        {
            _normale = collision.contacts[0].normal;

            if (collision.gameObject.tag == _obstaclesTag)
            {
                if (CheckObstacleInFront()) _healthScr.TakeDamage(200);
                else if (CheckObstacleInTheSide()) _healthScr.TakeDamage(200);
            }
            else if (collision.gameObject.tag == _enemyTag)
            {
                _healthScr.TakeDamage(1000);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _enemyTag)
            {
                _healthScr.TakeDamage(1000);
            }
            else if (other.gameObject.tag == _turnTag)
            {
                _isTurning = true;

                if (RunningLine < _roadManagerScr.CentralLine)
                {
                    _roadManagerScr.ChangeDirection(-1);
                    StartCoroutine(Turning(-1));
                    RunningLine += _roadManagerScr.CentralLine - 1;
                }
                else if (RunningLine > _roadManagerScr.CentralLine)
                {
                    _roadManagerScr.ChangeDirection(1);
                    StartCoroutine(Turning(1));
                    RunningLine -= _roadManagerScr.CentralLine - 1;

                }
                _roadManagerScr.FirstBuild();
            }
            else if (other.gameObject.tag == _startRoadPartTag)
            {
                CalculateLinesAmount();

            }
            else if (other.gameObject.tag == _endRoadPartTag)
            {
                _roadManagerScr.ReplaceRoadPart(false);
            }
            else if (other.gameObject.tag == _endRoadCrossTag)
            {
                _roadManagerScr.ReplaceRoadPart(true);
            }
            else if (other.gameObject.tag == _meleeCombatMode)
            {
                StartCoroutine(_playerMeleeCombatScr.Fight1());
            }
            else if (other.gameObject.tag == _skirmishMode)
            {
                StartCoroutine(_operatorScr.TransitionToTPM2());
            }
            else if (other.gameObject.tag == _coverMode)
            {
                other.gameObject.SetActive(false);
                StopMove();
                SetCoverCollider();
                _playerAnimationsScr.Cover(true);
                _roadManagerScr.StartSkirmish(true);
            }
            else if (other.gameObject.tag == _killAreaTag)
            {
                _roadManagerScr.StartSkirmish(false);
            }
            else if (other.gameObject.tag == _endSkirmish)
            {
                StartCoroutine(_operatorScr.TransitionToFPM());
            }
            else if (other.gameObject.tag == _stopControl)
            {
                _playerControllerScr.DisableControl();
            }
            else if (other.gameObject.tag == _continueControl)
            {
                _playerControllerScr.EnableControl();
                _playerControllerScr.EnableShooting();
            }
            else if (other.gameObject.tag == _stopShooting)
            {
                _playerControllerScr.DisableShooting();
            }
        }

        public Vector3 GetPlayerMoveDirection() => _moveDirection;
        public void StopMove() 
        {
            _isStopMove = true;
            _moveAudioSource.volume = 0;
        }
        public void ContinueMove() 
        {
            _isStopMove = false;
            _moveAudioSource.volume = 1;
        } 
    }
}