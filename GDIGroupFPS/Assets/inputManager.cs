using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class inputManager : MonoBehaviour
{
    public static inputManager instance;

    public bool MenuOpenCloseInput {  get; private set; }

    private PlayerInput _playerInput;

    private InputAction _menuOpenCloseAction;

    public bool MenuOpenCloseWebInput { get; private set; }


    private InputAction _menuOpenCloseWebAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _playerInput = GetComponent<PlayerInput>();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _menuOpenCloseWebAction = _playerInput.actions["MenuOpenCloseWeb"];
        }
        else
        {
            _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            MenuOpenCloseWebInput = _menuOpenCloseWebAction.WasPerformedThisFrame();
        }
        else
        {
            MenuOpenCloseInput = _menuOpenCloseAction.WasPerformedThisFrame();
        }

    }
}
