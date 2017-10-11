using UnityEngine;

namespace Rpg.Character
{
    public class ThirdPersonInput : MonoBehaviour
    {
        public delegate void OnEnableCursor(Vector3 position);
        public delegate void OnDisableCursor();
        public OnEnableCursor onEnableCursor;
        public OnDisableCursor onDisableCursor;

        [System.Serializable]
        public class OnUpdateEvent : UnityEngine.Events.UnityEvent<MeleeCombatInput> { }
        public enum GameplayInputStyle
        {
            ClickAndMove,
            DirectionalInput
        }
        public GameplayInputStyle gameplayInputStyle = GameplayInputStyle.ClickAndMove;
        protected InputDevice inputDevice
        {
            get
            {
                return InputControl.Instance.InputType;
            }
        }
        protected vThirdPersonCamera tpCamera;
        [HideInInspector]
        public bool changeCameraState;
        [HideInInspector]
        public string customCameraState;
        [HideInInspector]
        public string customlookAtPoint;
        [HideInInspector]
        public bool smoothCameraState;

        public bool lockCamera;
        [HideInInspector]
        public bool keepDirection;

        protected bool isRunning;

        public LayerMask clickMoveLayer = 1 << 0;

        protected Vector2 oldInput;
        [Header("Default Inputs")]
        public GenericInput horizontalInput = new GenericInput("Horizontal");
        public GenericInput verticallInput = new GenericInput("Vertical");
        public GenericInput leftMouseInput = new GenericInput("Fire1");
        public GenericInput rightMouseInput = new GenericInput("Fire2");
        public GenericInput jumpInput = new GenericInput("Space");

        [Header("Camera Settings")]
        public GenericInput rotateCameraXInput = new GenericInput("Mouse X", "RightAnalogHorizontal", "Mouse X");
        public GenericInput rotateCameraYInput = new GenericInput("Mouse Y", "RightAnalogVertical", "Mouse Y");
        public GenericInput cameraZoomInput = new GenericInput("Mouse ScrollWheel", "", "");

        public bool lockInput;
        [HideInInspector]
        public Vector3 cursorPoint;

        public ThirdPersonController character;
        [HideInInspector]
        public OnUpdateEvent onUpdateInput = new OnUpdateEvent();

        [HideInInspector]
        public HUDController hud;

        protected virtual void Start()
        {
            character = GetComponent<ThirdPersonController>();
            CharacterInit();
        }
        /// <summary>
        /// Input Event , Camera States 체크 
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (character == null || lockInput || Time.timeScale == 0)
            {
                return;
            }
            InputHandle();
            UpdateCameraStates();

        }
        /// <summary>
        /// 케릭터 물리모션 Move,Jump;
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (isRunning)
                MoveToPoint();
            character.AirControl();
            CameraInput();
        }

        protected virtual void Update()
        {
            character.UpdateMotor();
            character.UpdateAnimator();
            UpdateHUD();
        }

        protected virtual void CharacterInit()
        {
            if (character != null)
            {
                character.Init();
            }

            tpCamera = FindObjectOfType<vThirdPersonCamera>();
            if (tpCamera)
                tpCamera.SetMainTarget(this.transform);

            cursorPoint = transform.position;

            hud = HUDController.instance;
            if (hud != null)
                hud.Init(character);
        }

        #region Camera Methods
        public virtual void CameraInput()
        {
            if (!keepDirection)
                character.UpdateTargetDirection(Camera.main.transform);
            RotateWithCamera(Camera.main.transform);

            if (tpCamera == null || lockCamera)
                return;
            var Y = rotateCameraYInput.GetAxis();
            var X = rotateCameraXInput.GetAxis();
            var zoom = cameraZoomInput.GetAxis();

            tpCamera.RotateCamera(X, Y);
            tpCamera.Zoom(zoom);

            // change keedDirection from input diference
            if (keepDirection && Vector2.Distance(character.input, oldInput) > 0.2f)
                keepDirection = false;
        }

        protected virtual void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }

            if (changeCameraState && !character.isStrafing)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
            else if (character.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (character.isStrafing)
                tpCamera.ChangeState("Strafing", true);
            else
                tpCamera.ChangeState("Default", true);
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (character.isStrafing && !character.actions && !character.lockMovement)
            {
                // smooth align character with aim position               
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    character.RotateToTarget(tpCamera.lockTarget);
                }
                // rotate the camera around the character and align with when the char move
                else if (character.input != Vector2.zero)
                {
                    character.RotateWithAnotherTransform(cameraTransform);
                }
            }
        }

        #endregion

        protected virtual void InputHandle()
        {
            MoveCharacter();
            JumpInput();
        }

        protected virtual void MoveCharacter()
        {
            if (gameplayInputStyle == GameplayInputStyle.ClickAndMove)
            {
                ClickAndMove();
            }
            else
            {
                ControllerInput();
            }
        }

        protected virtual void ControllerInput()
        {
            // gets input from mobile           
            character.input.x = horizontalInput.GetAxis();
            character.input.y = verticallInput.GetAxis();
            // update oldInput to compare with current Input if keepDirection is true
            if (!keepDirection)
            {
                oldInput = character.input;
            }
        }

        protected virtual void ClickAndMove()
        {
            RaycastHit hit;

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clickMoveLayer))
                {
                    if (onEnableCursor != null)
                        onEnableCursor(hit.point);

                    isRunning = true;
                    cursorPoint = hit.point;
                }
            }
        }

        protected virtual void MoveToPoint()
        {
            if (gameplayInputStyle != GameplayInputStyle.ClickAndMove)
                return;

            var dir = (cursorPoint - transform.position).normalized;

            if (!NearPoint(cursorPoint, transform.position))
            {
                character.input = new Vector2(dir.x, dir.z);
            }
            else
            {
                character.input = Vector2.Lerp(character.input, Vector3.zero, 20f * Time.deltaTime);
                if (character.input.magnitude < 0.1 && isRunning)
                {
                    character.input = Vector2.zero;
                    isRunning = false;
                }

                if (onDisableCursor != null)
                    onDisableCursor();
            }
        }

        protected virtual bool NearPoint(Vector3 a, Vector3 b)
        {
            var _a = new Vector3(a.x, transform.position.y, a.z);
            var _b = new Vector3(b.x, transform.position.y, b.z);
            return Vector3.Distance(_a, _b) <= 0.25f;
        }

        protected virtual void JumpInput()
        {
            if (jumpInput.GetButtonDown())
            {
                character.Jump();
            }
        }

        #region HUD       

        public virtual void UpdateHUD()
        {
            if (hud == null)
                return;

            hud.UpdateHUD(character);
        }

        #endregion

        protected virtual void OnTriggerStay(Collider other)
        {
            character.CheckTriggers(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            character.CheckTriggerExit(other);
        }
    }
}
