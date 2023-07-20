using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class CharacterSFX : MonoBehaviour
    {
        [SerializeField]
        AudioClip[] _attackSfxList;
        [SerializeField]
        AudioClip _takeDamageSfx;
        [SerializeField]
        AudioClip _blockSfx;
        [SerializeField]
        AudioClip _hitSfx;

        CharacterCombat _combat;
        CharacterHealth _health;
        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _combat = GetComponent<CharacterCombat>();
            _health = GetComponent<CharacterHealth>();

            _combat.BlockedAttack += OnBlockedAttack;
            _combat.EnemyHit += OnEnemyHit;
            _combat.ComboAttackStarted += OnAttack;

            _health.DamageTaken += OnDamageTaken;
        }

        void OnDamageTaken(int damage)
        {
            _audioSource.PlayOneShot(_takeDamageSfx);
        }

        void OnBlockedAttack()
        {
            _audioSource.PlayOneShot(_blockSfx);
        }

        void OnEnemyHit()
        {
            _audioSource.PlayOneShot(_hitSfx);
        }

        void OnAttack(int comboIndex)
        {
            _audioSource.PlayOneShot(_attackSfxList[Random.Range(0, _attackSfxList.Length)]);
        }
    }
}