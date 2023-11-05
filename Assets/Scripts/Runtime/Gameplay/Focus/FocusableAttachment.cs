using UnityEngine;

namespace Recall.Gameplay
{
    public class FocusableAttachment : FocusableRigidbody
    {
        [SerializeField]
        Collider2D _targetAttachmentSlot;

        FocusableBehavior _behavior;

        protected override void Awake()
        {
            base.Awake();

            _behavior = GetComponent<FocusableBehavior>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider == _targetAttachmentSlot)
            {
                enabled = false;
                _rigidbody.isKinematic = true;
                transform.SetPositionAndRotation(collider.transform.position, Quaternion.identity);
                _behavior.enabled = true;
            }
        }
    }
}