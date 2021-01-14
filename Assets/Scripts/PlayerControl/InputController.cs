using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PlayerControl
{
    [Serializable]
    public class MoveInputEvent : UnityEvent<Vector2> { }
    [Serializable]
    public class LookInputEvent : UnityEvent<Vector2> { }

    public class InputController : MonoBehaviour
    {
        private Controls _controls;

        public MoveInputEvent moveInputEvent;
        public LookInputEvent lookInputEvent;
        // Start is called before the first frame update
        void Awake()
        {
            _controls = new Controls();
        }

        private void OnEnable()
        {
            _controls.Gameplay.Enable();
            _controls.Gameplay.Move.performed += OnMove;
            _controls.Gameplay.Move.canceled += OnMove;

            _controls.Gameplay.Look.performed += OnLookPerformed;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            moveInputEvent.Invoke(moveInput);
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            lookInputEvent.Invoke(context.ReadValue<Vector2>());
        }
    }
}
