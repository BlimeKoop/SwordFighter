//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/Game/Player/Input/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerMap"",
            ""id"": ""e1d4dcec-51d5-4432-bfff-edf37895c348"",
            ""actions"": [
                {
                    ""name"": ""Swing"",
                    ""type"": ""Value"",
                    ""id"": ""10595bcc-e292-4369-89e3-a0855b83f68b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8160818a-1027-4fc4-ba13-56b54c9ed323"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TogglePause"",
                    ""type"": ""Button"",
                    ""id"": ""f77cea21-c7ee-459f-bb5f-97ecaeafa2ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UnPause"",
                    ""type"": ""Button"",
                    ""id"": ""713c2628-3ef2-460f-838f-a4fbbadb30b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""08e1a7ac-f21c-4e5d-8236-ee6e3848b849"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Stab"",
                    ""type"": ""Button"",
                    ""id"": ""889e03d9-0aaf-4908-869e-6000faa8563c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""f15eb6d1-eba6-4f5c-94bd-731bd22fa551"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""cd230678-00f3-4789-9b59-c3e31657ca68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""14aafb02-ee69-433d-ad95-16dec70526de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""f2fff251-d9d2-4268-92c4-2030033915f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5cd612ed-29a1-4fd8-aabb-cb2c61f8c97c"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.08,y=0.08)"",
                    ""groups"": """",
                    ""action"": ""Swing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""36a99224-2c30-43aa-9e7d-30741dc156f6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""994b44f4-c279-42bf-9671-771240ff54b4"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""89b2a1ac-c2a7-4258-b29b-d81da48db4e4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e2885cba-38bc-4b7a-a067-90b189c4b684"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""dd838baa-e1b3-45cb-a161-dcbf7abaae7c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7f1e754b-6460-4c38-a3e5-06d5966c19d8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnPause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48b1418a-f32d-4a9a-a931-e52a0d382804"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnPause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84f38a3a-c042-4060-8f3e-f19fe3e5c321"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ddcbb8bf-daab-4776-a48a-86e2f97acf45"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4ecf137b-7768-4310-aba2-54e18c5aa666"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TogglePause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf9dd109-e542-4231-b749-a0c1edd77394"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21eae80e-525f-4258-8fe8-93ce6ef0697e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""75b16c25-71ef-4687-bf5a-103a7760f9b9"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e822e22f-ea30-474c-8936-f2b1c6b32f93"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82128e81-2f27-4da1-aaa8-eef7beb28887"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7920b72c-e26a-4e56-8975-1b25d6b4ac89"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerMap
        m_PlayerMap = asset.FindActionMap("PlayerMap", throwIfNotFound: true);
        m_PlayerMap_Swing = m_PlayerMap.FindAction("Swing", throwIfNotFound: true);
        m_PlayerMap_Move = m_PlayerMap.FindAction("Move", throwIfNotFound: true);
        m_PlayerMap_TogglePause = m_PlayerMap.FindAction("TogglePause", throwIfNotFound: true);
        m_PlayerMap_UnPause = m_PlayerMap.FindAction("UnPause", throwIfNotFound: true);
        m_PlayerMap_Block = m_PlayerMap.FindAction("Block", throwIfNotFound: true);
        m_PlayerMap_Stab = m_PlayerMap.FindAction("Stab", throwIfNotFound: true);
        m_PlayerMap_Restart = m_PlayerMap.FindAction("Restart", throwIfNotFound: true);
        m_PlayerMap_Quit = m_PlayerMap.FindAction("Quit", throwIfNotFound: true);
        m_PlayerMap_Crouch = m_PlayerMap.FindAction("Crouch", throwIfNotFound: true);
        m_PlayerMap_Jump = m_PlayerMap.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerMap
    private readonly InputActionMap m_PlayerMap;
    private List<IPlayerMapActions> m_PlayerMapActionsCallbackInterfaces = new List<IPlayerMapActions>();
    private readonly InputAction m_PlayerMap_Swing;
    private readonly InputAction m_PlayerMap_Move;
    private readonly InputAction m_PlayerMap_TogglePause;
    private readonly InputAction m_PlayerMap_UnPause;
    private readonly InputAction m_PlayerMap_Block;
    private readonly InputAction m_PlayerMap_Stab;
    private readonly InputAction m_PlayerMap_Restart;
    private readonly InputAction m_PlayerMap_Quit;
    private readonly InputAction m_PlayerMap_Crouch;
    private readonly InputAction m_PlayerMap_Jump;
    public struct PlayerMapActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerMapActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Swing => m_Wrapper.m_PlayerMap_Swing;
        public InputAction @Move => m_Wrapper.m_PlayerMap_Move;
        public InputAction @TogglePause => m_Wrapper.m_PlayerMap_TogglePause;
        public InputAction @UnPause => m_Wrapper.m_PlayerMap_UnPause;
        public InputAction @Block => m_Wrapper.m_PlayerMap_Block;
        public InputAction @Stab => m_Wrapper.m_PlayerMap_Stab;
        public InputAction @Restart => m_Wrapper.m_PlayerMap_Restart;
        public InputAction @Quit => m_Wrapper.m_PlayerMap_Quit;
        public InputAction @Crouch => m_Wrapper.m_PlayerMap_Crouch;
        public InputAction @Jump => m_Wrapper.m_PlayerMap_Jump;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMapActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerMapActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Add(instance);
            @Swing.started += instance.OnSwing;
            @Swing.performed += instance.OnSwing;
            @Swing.canceled += instance.OnSwing;
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @TogglePause.started += instance.OnTogglePause;
            @TogglePause.performed += instance.OnTogglePause;
            @TogglePause.canceled += instance.OnTogglePause;
            @UnPause.started += instance.OnUnPause;
            @UnPause.performed += instance.OnUnPause;
            @UnPause.canceled += instance.OnUnPause;
            @Block.started += instance.OnBlock;
            @Block.performed += instance.OnBlock;
            @Block.canceled += instance.OnBlock;
            @Stab.started += instance.OnStab;
            @Stab.performed += instance.OnStab;
            @Stab.canceled += instance.OnStab;
            @Restart.started += instance.OnRestart;
            @Restart.performed += instance.OnRestart;
            @Restart.canceled += instance.OnRestart;
            @Quit.started += instance.OnQuit;
            @Quit.performed += instance.OnQuit;
            @Quit.canceled += instance.OnQuit;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
        }

        private void UnregisterCallbacks(IPlayerMapActions instance)
        {
            @Swing.started -= instance.OnSwing;
            @Swing.performed -= instance.OnSwing;
            @Swing.canceled -= instance.OnSwing;
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @TogglePause.started -= instance.OnTogglePause;
            @TogglePause.performed -= instance.OnTogglePause;
            @TogglePause.canceled -= instance.OnTogglePause;
            @UnPause.started -= instance.OnUnPause;
            @UnPause.performed -= instance.OnUnPause;
            @UnPause.canceled -= instance.OnUnPause;
            @Block.started -= instance.OnBlock;
            @Block.performed -= instance.OnBlock;
            @Block.canceled -= instance.OnBlock;
            @Stab.started -= instance.OnStab;
            @Stab.performed -= instance.OnStab;
            @Stab.canceled -= instance.OnStab;
            @Restart.started -= instance.OnRestart;
            @Restart.performed -= instance.OnRestart;
            @Restart.canceled -= instance.OnRestart;
            @Quit.started -= instance.OnQuit;
            @Quit.performed -= instance.OnQuit;
            @Quit.canceled -= instance.OnQuit;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
        }

        public void RemoveCallbacks(IPlayerMapActions instance)
        {
            if (m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerMapActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerMapActions @PlayerMap => new PlayerMapActions(this);
    public interface IPlayerMapActions
    {
        void OnSwing(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnTogglePause(InputAction.CallbackContext context);
        void OnUnPause(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnStab(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
