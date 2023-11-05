using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FocusableRigidbody : FocusableBase
    {
        [SerializeField]
        Vector2 _sfxVolumeRange = Vector2.one;
        [SerializeField]
        Vector2 _sfxPitchRange = Vector2.one;
        [SerializeField, Range(.05f, .5f)]
        float _sfxMinInterval = .2f;

        protected Rigidbody2D _rigidbody;

        AudioSource _audioSource;
        float _sfxReadyTimestamp;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnSelect()
        {
            base.OnSelect();
            _rigidbody.gravityScale = 0;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public override void OnUnselect()
        {
            base.OnUnselect();
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            _rigidbody.gravityScale = 1;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _rigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
            _rigidbody.interpolation = RigidbodyInterpolation2D.None;
        }

        public override void OnDrag(Vector2 delta, Vector2 finalPosition)
        {
            _rigidbody.MovePosition(finalPosition);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (Time.time > _sfxReadyTimestamp)
            {
                _audioSource.pitch = UnityEngine.Random.Range(_sfxPitchRange.x, _sfxPitchRange.y);
                _audioSource.PlayOneShot(_audioSource.clip, UnityEngine.Random.Range(_sfxVolumeRange.x, _sfxVolumeRange.y));
                _sfxReadyTimestamp = Time.time + _sfxMinInterval;
            }
        }
    }
}