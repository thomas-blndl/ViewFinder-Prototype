using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool picture;
        public bool usePicture;
        public bool lookPicture;
        public bool activateCamera;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        public void OnPicture(InputValue value)
        {
            PictureInput(value.isPressed);
        }
        public void OnUsePicture(InputValue value)
        {
            UsePictureInput(value.isPressed);
        }
        public void OnLookPicture(InputValue value)
        {
            LookPictureInput(value.isPressed);
        }
        public void OnActivateCamera(InputValue value)
        {
            ActivateCameraInput(value.isPressed);
        }

#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
        public void PictureInput(bool newPictureState)
        {
            picture = newPictureState;
        }
        public void UsePictureInput(bool newUsePictureState)
        {
            usePicture = newUsePictureState;
        }
        public void LookPictureInput(bool newLookPictureState)
        {
            lookPicture = newLookPictureState;
        }
        public void ActivateCameraInput(bool newActivateCameraState)
        {
            activateCamera = newActivateCameraState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}