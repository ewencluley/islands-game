using System;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [Range(1, 100)] public float speed = 1f;
        public Transform head;
        public bool invertY;
        
        private Vector2 _move;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void SetMovement(Vector2 move)
        {
            this._move = move;
        }

        public void SetLook(Vector2 look)
        {
            var horizontalDelta = Quaternion.Euler(0f, look.x, 0f);
            var verticalDelta = Quaternion.Euler(invertY ? -look.y : look.y, 0f, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * horizontalDelta);
            head.transform.rotation *= verticalDelta;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(transform.position + ((transform.forward * _move.y + transform.right * _move.x) * (speed * Time.deltaTime)));
        }
    }
}