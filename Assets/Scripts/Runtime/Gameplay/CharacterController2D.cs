using Recall.Gameplay.Interfaces;
using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class CharacterController2D : MonoBehaviour, ISpriteFlippable
    {
        struct GroundDetectInfo
        {
            public Vector2 Position;
            public float Radius;
        }

        struct WallDetectInfo
        {
            public Vector2 Position;
            public Vector2 Size;
        }

        [Flags]
        enum WallSide : byte
        {
            None = 0,
            LeftSide = 1,
            RightSide = 2
        }

        public event Action<bool> GroundedStateChanged;
        public event Action<bool> FlippedStateChanged;
        public event Action Jumped;

        public float HorizontalInput => _horizontalInput;
        public bool IsGrounded => _isGrounded;
        public bool IsFlipped => _isFlipped;

        [SerializeField]
        LayerMask _groundLayers;
        [SerializeField, Range(-.1f, .1f)]
        float _groundDetectOffset;
        [SerializeField, Range(-.1f, .1f)]
        float _groundDetectRadiusOffset;
        [SerializeField]
        LayerMask _wallLayers;
        [SerializeField, Range(0, .1f)]
        float _wallDetectWidth;
        [SerializeField, Range(-.5f, .5f)]
        float _wallDetectHeightOffset;
        [SerializeField, Range(0, 10)]
        float _speed = 4;
        [SerializeField, Range(0, 20)]
        float _jumpForce = 8.5f;

        readonly Collider2D[] _overlapCache = new Collider2D[1];

        CapsuleCollider2D _capsuleCollider;
        CharacterCombat _characterCombat;
        Rigidbody2D _rigidbody;
        float _speedMultiplier = 1;
        float _horizontalInput;
        bool _wasGroundedLastFrame;
        bool _wasFlippedLastFrame;
        bool _isGrounded;
        bool _isFlipped;
        bool _jumpInput;

        void Awake()
        {
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _characterCombat = GetComponent<CharacterCombat>();
            _rigidbody = GetComponent<Rigidbody2D>();

            _characterCombat.ComboAttackStarted += OnComboAttackStarted;
            _characterCombat.ComboEnded += OnComboEnded;
            _characterCombat.DefendingStateChanged += OnDefendingStateChanged;

            if (TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.RespawnedAt += OnRespawned;
        }

        void FixedUpdate()
        {
            ValidateGroundedState();
            ValidateFlippedState(_horizontalInput);

            var velocity = new Vector2(_horizontalInput * _speed * _speedMultiplier, _rigidbody.velocity.y);

            if (_isGrounded)
                FixedGroundedMove(ref velocity);
            else
                FixedAirMove(ref velocity);

            _rigidbody.velocity = velocity;

            ResetInputs();
        }

        public void SetInputs(float horizontalInput, bool jumpInput)
        {
            _horizontalInput = horizontalInput;
            _jumpInput = jumpInput || _jumpInput;
        }

        public void WarpToPosition(Vector3 position)
        { 
            _rigidbody.position = position;
        }
        
        void FixedGroundedMove(ref Vector2 velocity)
        {
            if (_jumpInput)
            {
                velocity.y = _jumpForce;
                _jumpInput = false;
                Jumped?.Invoke();
            }
        }

        void FixedAirMove(ref Vector2 velocity)
        {
            _jumpInput = false;
            if (IsWallSliding(out var wallSlideFlags))
            {
                if (velocity.x > 0 && wallSlideFlags.HasFlag(WallSide.RightSide) || velocity.x < 0 && wallSlideFlags.HasFlag(WallSide.LeftSide))
                    velocity.x = 0;
            }
        }

        void ValidateGroundedState()
        {
            var groundDetectInfo = GetGroundDetectInfo();
            _isGrounded = Physics2D.OverlapCircleNonAlloc(groundDetectInfo.Position, groundDetectInfo.Radius, _overlapCache, _groundLayers.value) > 0;

            if (_wasGroundedLastFrame != _isGrounded)
                GroundedStateChanged?.Invoke(_isGrounded);

            _wasGroundedLastFrame = _isGrounded;
        }

        void ValidateFlippedState(float input)
        {
            if (input != 0)
                _isFlipped = input < 0;

            if (_wasFlippedLastFrame != _isFlipped)
                FlippedStateChanged?.Invoke(_isFlipped);

            _wasFlippedLastFrame = _isFlipped;
        }

        void ResetInputs()
        {
            _horizontalInput = 0;
        }

        void OnRespawned(Vector2 position)
        {
            WarpToPosition(position);
            ValidateFlippedState(1);
        }

        void OnComboAttackStarted(int comboIndex)
        {
            if (comboIndex != 1)
                return;

            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector2.zero;
            enabled = false;
        }

        void OnComboEnded()
        {
            enabled = true;
            _rigidbody.isKinematic = false;
        }

        void OnDefendingStateChanged(bool isDefending)
        {
            _speedMultiplier = isDefending ? 0 : 1;
        }

        GroundDetectInfo GetGroundDetectInfo()
        {
            var radius = (_groundDetectRadiusOffset + _capsuleCollider.size.x) / 2;
            return new GroundDetectInfo
            {
                Position = new Vector2(transform.position.x, transform.position.y - _groundDetectOffset + radius),
                Radius = radius
            };
        }

        WallDetectInfo GetWallDetectInfo(WallSide side)
        {
            var radius = _capsuleCollider.size.x;
            return new WallDetectInfo
            {
                Position = new Vector2(transform.position.x + (side == WallSide.RightSide ? radius + _wallDetectWidth : -radius - _wallDetectWidth) / 2, transform.position.y + _capsuleCollider.size.y / 2),
                Size = new Vector2(_wallDetectWidth, _capsuleCollider.size.y - radius + _wallDetectHeightOffset)
            };
        }

        bool IsWallSliding(out WallSide wallSlideFlags)
        {
            WallDetectInfo info;
            wallSlideFlags = WallSide.None;
            for (int i = 0; i < 2; i++)
            {
                info = GetWallDetectInfo(i == 0 ? WallSide.RightSide : WallSide.LeftSide);
                if (Physics2D.OverlapBoxNonAlloc(info.Position, info.Size, 0, _overlapCache, _wallLayers.value) > 0)
                    wallSlideFlags |= i == 0 ? WallSide.RightSide : WallSide.LeftSide;
            }
            return wallSlideFlags == WallSide.None;
        }

        void OnDrawGizmosSelected()
        {
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            Gizmos.color = Color.cyan;
            var groundDetectInfo = GetGroundDetectInfo();
            Gizmos.DrawWireSphere(groundDetectInfo.Position, groundDetectInfo.Radius);

            WallDetectInfo info;
            for (int i = 0; i < 2; i++)
            {
                info = GetWallDetectInfo(i == 0 ? WallSide.RightSide : WallSide.LeftSide);
                Gizmos.DrawWireCube(info.Position, info.Size);
            }
        }
    }
}