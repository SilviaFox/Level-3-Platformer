using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputMaster inputActions;
    public static InputManager instance;
    public static bool jumpHeld;

    private void OnEnable()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        inputActions = new InputMaster();
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Jump.started += ctx => {PlayerController.current.Jump(); jumpHeld = true;};
        inputActions.Gameplay.Jump.canceled += ctx => jumpHeld = false;

        inputActions.Gameplay.Dash.started += ctx =>{if (!PlayerController.current.isAttacking) PlayerController.current.StartCoroutine(PlayerController.current.Dash());};
        inputActions.Gameplay.Dash.canceled += ctx => PlayerController.current.ableToDash = false;

        inputActions.Gameplay.Shoot.started += ctx => {
            if (!PlayerController.current.isAttacking)
                PlayerController.current.Shoot(0);
                PlayerController.current.isCharging = true;
                PlayerController.current.StartCoroutine(PlayerController.current.Charge());
            };
        inputActions.Gameplay.Shoot.canceled += ctx => PlayerController.current.isCharging = false;

        inputActions.Gameplay.Attack.started += ctx => {
            if(!PlayerController.current.isAttacking) PlayerController.current.StartCoroutine(PlayerController.current.Attack());};
        
        inputActions.Gameplay.DebugReset.started += ctx => UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        PlayerController.moveDirection = inputActions.Gameplay.Movement.ReadValue<float>();
    }

}
