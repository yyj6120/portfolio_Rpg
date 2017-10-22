using UnityEngine;
using System;
namespace Rpg.Character
{
    [HideInInspector]
    public enum InputDevice
    {
        MouseKeyboard,
        Joystick,
        Mobile
    };
    public class InputControl : MonoBehaviour
    {
        public delegate void OnChangeInputType(InputDevice type);
        public event OnChangeInputType onChangeInputType;
        private static InputControl instance;
        public static InputControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<InputControl>();
                    if (instance == null)
                    {
                        new GameObject("InputType", typeof(InputControl));
                        return InputControl.instance;
                    }
                }
                return instance;
            }
        }
        private InputDevice inputType = InputDevice.MouseKeyboard;
        public InputDevice InputType
        {
            get
            {
                return inputType;
            }

            set
            {
                inputType = value;
            }
        }
    }
    [Serializable]
    public class GenericInput
    {
        public GenericInput(string keyboard)
        {
            this.keyboard = keyboard;
        }
        public GenericInput(string keyboard, string joystick, string mobile)
        {
            this.keyboard = keyboard;
            this.joystick = joystick;
            this.mobile = mobile;
        }
        public GenericInput(string keyboard, bool keyboardAxis, string joystick, bool joystickAxis, string mobile, bool mobileAxis)
        {
            this.keyboard = keyboard;
            this.keyboardAxis = keyboardAxis;
            this.joystick = joystick;
            this.joystickAxis = joystickAxis;
            this.mobile = mobile;
            this.mobileAxis = mobileAxis;
        }
        protected InputDevice inputDevice
        {
            get
            {
                return InputControl.Instance.InputType;
            }
        }
        public bool useInput = true;
        [SerializeField]
        private bool isAxisInUse;

        [SerializeField]
        private string keyboard;
        [SerializeField]
        private bool keyboardAxis;
        [SerializeField]
        private string joystick;
        [SerializeField]
        private bool joystickAxis;
        [SerializeField]
        private string mobile;
        [SerializeField]
        private bool mobileAxis;

        [SerializeField]
        private bool joystickAxisInvert;
        [SerializeField]
        private bool keyboardAxisInvert;
        [SerializeField]
        private bool mobileAxisInvert;

        private float buttomTimer;
        private bool inButtomTimer;
        private float multTapTimer;
        private int multTapCounter;

        public string buttonName
        {
            get
            {
                if (InputControl.Instance != null)
                {
                    if (InputControl.Instance.InputType == InputDevice.MouseKeyboard)
                    {
                        return keyboard.ToString();
                    }
                    else if (InputControl.Instance.InputType == InputDevice.Joystick)
                    {
                        return joystick;
                    }
                    else return mobile;
                }
                return string.Empty;
            }
        }

        public bool isKey
        {
            get
            {
                if (InputControl.Instance != null)
                {
                    if (System.Enum.IsDefined(typeof(KeyCode), buttonName))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool isAxis
        {
            get
            {
                bool value = false;
                switch (inputDevice)
                {
                    case InputDevice.Joystick:
                        value = joystickAxis;
                        break;
                    case InputDevice.MouseKeyboard:
                        value = keyboardAxis;
                        break;
                    case InputDevice.Mobile:
                        value = mobileAxis;
                        break;
                }
                return value;
            }
        }

        public bool isAxisInvert
        {
            get
            {
                bool value = false;
                switch (inputDevice)
                {
                    case InputDevice.Joystick:
                        value = joystickAxisInvert;
                        break;
                    case InputDevice.MouseKeyboard:
                        value = keyboardAxisInvert;
                        break;
                    case InputDevice.Mobile:
                        value = mobileAxisInvert;
                        break;
                }
                return value;
            }
        }

        public KeyCode key
        {
            get
            {
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), buttonName);
            }
        }

        /// <summary>
        /// Get Axis
        /// </summary>
        /// <returns></returns>
        public float GetAxis()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || isKey) return 0;

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                return CrossPlatformInputManager.GetAxis(this.buttonName);
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                return Input.GetAxis(this.buttonName);
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                return Input.GetAxis(this.buttonName);
            }
            return 0;
        }

        public float GetAxisRaw()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || isKey) return 0;

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                return CrossPlatformInputManager.GetAxisRaw(this.buttonName);
#endif
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                return Input.GetAxisRaw(this.buttonName);
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                return Input.GetAxisRaw(this.buttonName);
            }
            return 0;
        }

        public bool GetAxisButton(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxisInvert) value *= -1f;
            if (value > 0)
            {
                return GetAxisRaw() >= value;
            }
            else if (value < 0)
            {
                return GetAxisRaw() <= value;
            }
            return false;
        }

        public bool GetAxisButtonDown(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName))
            {
                return false;
            }
            if (isAxisInvert)
            {
                value *= -1f;
            }
            if (value > 0)
            {
                if (!isAxisInUse && GetAxisRaw() >= value)
                {
                    isAxisInUse = true;
                    return true;
                }
                else if (isAxisInUse && GetAxisRaw() == 0)
                {
                    isAxisInUse = false;
                }
            }
            else if (value < 0)
            {
                if (!isAxisInUse && GetAxisRaw() <= value)
                {
                    isAxisInUse = true;
                    return true;
                }
                else if (isAxisInUse && GetAxisRaw() == 0)
                {
                    isAxisInUse = false;
                }
            }
            return false;
        }

        public bool GetAxisButtonUp()
        {
            if (isAxisInUse && GetAxisRaw() == 0)
            {
                isAxisInUse = false;
                return true;
            }
            else if (!isAxisInUse && GetAxisRaw() != 0)
            {
                isAxisInUse = true;
            }
            return false;
        }
        /// <summary>
        /// Get Button
        /// </summary>
        /// <returns></returns>
        public bool GetButton()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButton();

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                if (CrossPlatformInputManager.GetButton(this.buttonName))
#endif
                return true;
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKey(key))
                        return true;
                }
                else
                {
                    if (Input.GetButton(this.buttonName))
                        return true;
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButton(this.buttonName))
                    return true;
            }
            return false;
        }

        public bool GetButtonDown()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName))
            {
                return false;
            }
            if (isAxis)
            {
                return GetAxisButtonDown();
            }
            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                if (CrossPlatformInputManager.GetButtonDown(this.buttonName))
#endif
                return true;
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKeyDown(key))
                    {
                        return true;
                    }
                }
                else
                {
                    if (Input.GetButtonDown(this.buttonName))
                    {
                        return true;
                    }
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButtonDown(this.buttonName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Button Up
        /// </summary>
        /// <returns></returns>
        public bool GetButtonUp()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName))
            {
                return false;
            }
            if (isAxis)
            {
                return GetAxisButtonUp();
            }
            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
#if MOBILE_INPUT
                if (CrossPlatformInputManager.GetButtonUp(this.buttonName))
#endif
                return true;
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKeyUp(key))
                        return true;
                }
                else
                {
                    if (Input.GetButtonUp(this.buttonName))
                        return true;
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButtonUp(this.buttonName))
                    return true;
            }
            return false;
        }

        bool IsButtonAvailable(string btnName)
        {
            if (!useInput)
            {
                return false;
            }
            try
            {
                if (isKey)
                {
                    return true;
                }
                Input.GetButton(buttonName);
                return true;
            }
            catch (System.Exception exc)
            {
                Debug.LogWarning(" Failure to try access button :" + buttonName + "\n" + exc.Message);
                return false;
            }
        }

    }


}
