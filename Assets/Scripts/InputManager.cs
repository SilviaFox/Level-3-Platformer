using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputMaster inputActions;

    public static bool jumpHeld;

    private void Start()
    {
        if (inputActions == null)
            inputActions = new InputMaster();

        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Jump.started += ctx => {PlayerController.current.Jump(); jumpHeld = true;};
        inputActions.Gameplay.Jump.canceled += ctx => jumpHeld = false;

        inputActions.Gameplay.Dash.started += ctx =>{if (!PlayerController.current.isAttacking) StartCoroutine(PlayerController.current.Dash());};
        inputActions.Gameplay.Dash.canceled += ctx => PlayerController.current.ableToDash = false;

        inputActions.Gameplay.Shoot.started += ctx => {
            if (!PlayerController.current.isAttacking)
                PlayerController.current.Shoot(0);
                PlayerController.current.isCharging = true;
                StartCoroutine(PlayerController.current.Charge());
            };
        inputActions.Gameplay.Shoot.canceled += ctx => PlayerController.current.isCharging = false;

        inputActions.Gameplay.Attack.started += ctx => {
            if(!PlayerController.current.isAttacking) StartCoroutine(PlayerController.current.Attack());};
    }

    void Update()
    {
        PlayerController.moveDirection = inputActions.Gameplay.Movement.ReadValue<float>();
    }

}
