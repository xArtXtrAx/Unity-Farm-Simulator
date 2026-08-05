using UnityEngine;
using UnityEngine.InputSystem;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class UnifiedMovementInput : MonoBehaviour
    {
        private TopDownPlayerMotor motor;

        public Vector2 LastRawInput { get; private set; }

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
        }

        private void Update()
        {
            LastRawInput = ReadRawMovement();
            motor.SetDesiredInput(LastRawInput);
        }

        private void OnDisable()
        {
            if (motor != null)
            {
                motor.Stop();
            }
        }

        public Vector2 ReadRawMovement()
        {
            Vector2 keyboardMovement = ReadKeyboardMovement();
            Vector2 gamepadMovement = ReadGamepadMovement();

            return gamepadMovement.sqrMagnitude > keyboardMovement.sqrMagnitude
                ? gamepadMovement
                : keyboardMovement;
        }

        private static Vector2 ReadKeyboardMovement()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontal -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontal += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                vertical -= 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                vertical += 1f;
            }

            return new Vector2(horizontal, vertical);
        }

        private static Vector2 ReadGamepadMovement()
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad == null && Gamepad.all.Count > 0)
            {
                gamepad = Gamepad.all[0];
            }

            if (gamepad == null)
            {
                return Vector2.zero;
            }

            Vector2 stick = gamepad.leftStick.ReadValue();
            Vector2 dpad = gamepad.dpad.ReadValue();
            return dpad.sqrMagnitude > stick.sqrMagnitude ? dpad : stick;
        }
    }
}
