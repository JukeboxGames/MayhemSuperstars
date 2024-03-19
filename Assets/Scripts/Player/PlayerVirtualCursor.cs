using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;

// El cursor virtual solo se usará en el modo editor
// El resto de menus debe ser navegable
public class VirtualCursor : MonoBehaviour
{
    #region references
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] public RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Camera myCamera;
    [SerializeField] private float padding = 60f;
    #endregion

    #region Input Functions
    Vector2 input_Movement;
    bool input_rotateLeft, input_rotateRight, input_place;
    #endregion

    public Mouse virtualMouse;
    public int playerNumber = -1;
    Vector2 newPosition;
    public bool stopRecordingInput = false;

    private void OnEnable() {
        if (!(playerInput.currentControlScheme == "Keyboard")) {
            if (virtualMouse == null) { 
                virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
            }
            else if (!virtualMouse.added){
                InputSystem.AddDevice(virtualMouse);
            }

            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

            cursorTransform.gameObject.SetActive(true);

            if (cursorTransform != null) {
                Vector2 position = cursorTransform.anchoredPosition;
                InputState.Change(virtualMouse.position, position);
            }

            InputSystem.onAfterUpdate += UpdateMotion;

            InitializeInput();

            AnchorCursor(Vector2.zero);
        } else {
            cursorTransform.gameObject.SetActive(false);
        }
    }

    private void OnDisable() {
        if (virtualMouse != null) {
            InputSystem.RemoveDevice(virtualMouse);
            InputSystem.onAfterUpdate -= UpdateMotion;
            StopInput();
        }
    }

    private void UpdateMotion() {
        if (virtualMouse == null || stopRecordingInput){
            return;
        }

        Vector2 deltaValue = input_Movement;
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        newPosition = currentPosition + deltaValue;

        LimitBorder();

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        AnchorCursor(newPosition);
    }

    private void LimitBorder () {
        newPosition.x = Mathf.Clamp(newPosition.x, myCamera.pixelWidth + padding, myCamera.pixelWidth * 2f - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, 0 + padding, myCamera.pixelHeight - padding);
    }

    private void AnchorCursor(Vector2 position){
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : myCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    #region Input Functions
        // Aun no pienso si vamos a gestionar aqui el input para el modo editor
        void InitializeInput(){
            playerInput.actions["Movement"].performed += OnPoint;
            playerInput.actions["Rotate Left"].performed += OnRotateLeft;
            playerInput.actions["Rotate Right"].performed += OnRotateRight;
            playerInput.actions["Place"].performed += OnPoint;

            playerInput.actions["Rotate Left"].canceled += OnRotateLeft;
            playerInput.actions["Rotate Right"].canceled += OnRotateRight;
            playerInput.actions["Place"].canceled += OnPoint;
        }

        void StopInput () {
            playerInput.actions["Movement"].performed -= OnPoint;
            playerInput.actions["Rotate Left"].performed -= OnRotateLeft;
            playerInput.actions["Rotate Right"].performed -= OnRotateRight;
            playerInput.actions["Place"].performed -= OnPoint;

            playerInput.actions["Rotate Left"].canceled -= OnRotateLeft;
            playerInput.actions["Rotate Right"].canceled -= OnRotateRight;
            playerInput.actions["Place"].canceled -= OnPoint;
        }   


        public void OnPoint(InputAction.CallbackContext context){
            input_Movement = context.ReadValue<Vector2>();
        }

        public void OnRotateLeft(InputAction.CallbackContext context){
            input_rotateLeft = context.ReadValue<bool>();
        }

        public void OnRotateRight(InputAction.CallbackContext context){
            input_rotateRight = context.ReadValue<bool>();
        }

        public void OnPlace(InputAction.CallbackContext context){
            input_place = context.ReadValue<bool>();
        }


    #endregion
}
