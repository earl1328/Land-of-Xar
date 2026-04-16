using System;
using UnityEngine;

namespace VirController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [Header("Stats")]
        [SerializeField] private ScriptableStats _stats;

        [Header("Audio")]
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip _jumpSfx;
        [SerializeField] private AudioClip _doubleJumpSfx;

        [Header("Footsteps")]
        [SerializeField] private AudioClip _walkSfx;
        [SerializeField] private float _footstepInterval = 0.35f;

        private float _footstepTimer;

        private Rigidbody2D _rb;
        private Collider2D _col;

        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        private RaycastHit2D[] _hits = new RaycastHit2D[5];

        #region DASH
        private bool _isDashing;
        private float _dashTime;
        private float _nextDashTime;
        #endregion

        #region DOUBLE JUMP
        private int _jumpCount;
        #endregion

        #region Interface
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Dash = Input.GetKeyDown(KeyCode.LeftShift),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
                _jumpToConsume = true;
        }

        private void FixedUpdate()
        {
            CheckCollisions();
            HandleDash();

            if (!_isDashing)
            {
                HandleJump();
                HandleDirection();
                HandleGravity();
                HandleFootsteps(); // ✅ FOOTSTEPS ADDED HERE
            }

            ApplyMovement();
        }

        #region AUDIO
        private void PlaySfx(AudioClip clip)
        {
            if (clip == null || _audio == null) return;
            _audio.PlayOneShot(clip);
        }
        #endregion

        #region FOOTSTEPS
        private void HandleFootsteps()
        {
            if (_grounded && Mathf.Abs(_frameVelocity.x) > 0.1f)
            {
                _footstepTimer -= Time.fixedDeltaTime;

                if (_footstepTimer <= 0f)
                {
                    PlaySfx(_walkSfx);
                    _footstepTimer = _footstepInterval;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }
        #endregion

        #region DASH
        private void HandleDash()
        {
            if (!_isDashing && _frameInput.Dash && Time.time >= _nextDashTime)
            {
                _isDashing = true;
                _dashTime = _stats.DashDuration;
                _nextDashTime = Time.time + _stats.DashCooldown;

                float dir = _frameInput.Move.x;

                if (dir == 0)
                    dir = transform.localScale.x >= 0 ? 1 : -1;
                else
                    dir = Mathf.Sign(dir);

                _frameVelocity = new Vector2(dir * _stats.DashSpeed, 0f);
            }

            if (_isDashing)
            {
                _dashTime -= Time.fixedDeltaTime;

                if (_dashTime <= 0f)
                    _isDashing = false;
            }
        }
        #endregion

        #region COLLISIONS
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(~_stats.PlayerLayer);
            filter.useTriggers = false;

            bool groundHit = false;
            bool ceilingHit = false;

            int groundHits = _col.Cast(Vector2.down, filter, _hits, _stats.GrounderDistance);
            for (int i = 0; i < groundHits; i++)
                if (_hits[i].normal.y > 0.5f) groundHit = true;

            int ceilingHits = _col.Cast(Vector2.up, filter, _hits, _stats.GrounderDistance);
            for (int i = 0; i < ceilingHits; i++)
                if (_hits[i].normal.y < -0.5f) ceilingHit = true;

            if (ceilingHit)
                _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;

                _jumpCount = 0;

                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }
        #endregion

        #region JUMP
        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0)
                _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote)
            {
                ExecuteJump();
                _jumpCount = 1;
                PlaySfx(_jumpSfx);
            }
            else if (_jumpCount < 2)
            {
                ExecuteJump();
                _jumpCount++;
                PlaySfx(_doubleJumpSfx);
            }

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;

            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }
        #endregion

        #region MOVEMENT
        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var decel = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, decel * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(
                    _frameVelocity.x,
                    _frameInput.Move.x * _stats.MaxSpeed,
                    _stats.Acceleration * Time.fixedDeltaTime
                );
            }
        }
        #endregion

        #region GRAVITY
        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var gravity = _stats.FallAcceleration;

                if (_endedJumpEarly && _frameVelocity.y > 0)
                    gravity *= _stats.JumpEndEarlyGravityModifier;

                _frameVelocity.y = Mathf.MoveTowards(
                    _frameVelocity.y,
                    -_stats.MaxFallSpeed,
                    gravity * Time.fixedDeltaTime
                );
            }
        }
        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null)
                Debug.LogWarning("Assign ScriptableStats to PlayerController", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public bool Dash;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        event Action<bool, float> GroundedChanged;
        event Action Jumped;
        Vector2 FrameInput { get; }
    }
}
