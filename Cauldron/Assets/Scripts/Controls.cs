// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CauldronCodebase
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""General"",
            ""id"": ""55cbab9b-28a9-4023-bbf2-b6df599e58ca"",
            ""actions"": [
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""f87eac4a-11ac-4c8a-a46f-0e1cd193ae01"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BookToggle"",
                    ""type"": ""Button"",
                    ""id"": ""b109a3d6-0148-4923-a8d5-6c90df1b42ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BookNavigate"",
                    ""type"": ""Value"",
                    ""id"": ""5ce93e55-d2f6-4c8d-9518-194dd1d59c5c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7ce30173-58a6-481e-980d-0854775c7f95"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cfef83c2-baea-4651-af99-86a5a328245f"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68ca8468-7d48-444f-b9d6-f466c2e096a7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54fa201f-f8d4-47ac-af80-2d4fa2070afa"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""576926e1-89e7-4b12-8a1f-1793baa68a32"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""wasd"",
                    ""id"": ""9c2e8732-f0d6-4326-aa29-2cf5a7d9581f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e71ef31f-f988-47df-8fc1-95ad2c97a958"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9b81b220-01ad-452a-a6e0-bdd68dfa1581"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""958b693d-29ea-4c7c-9a4d-7fc3e3db7208"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0e266207-48f4-43a3-946d-256b9e2844fb"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""arrows"",
                    ""id"": ""1e3b357a-ad87-4622-babe-106187c4a19f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""55020d55-66e2-4a3c-89f4-3b3069b692ca"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ed7b136a-1c38-489d-a9af-b52082511d4f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c9485fcd-5364-49fe-961f-ee248f5f3bd8"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2eb12dea-d8b2-4b14-88f6-6626e95ef042"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BookNavigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""90e0a50b-57cb-4492-bdf5-509fa5eb80ce"",
            ""actions"": [
                {
                    ""name"": ""ClearSave"",
                    ""type"": ""Button"",
                    ""id"": ""c95c986b-ea29-40fb-a923-e6fb0d4a3f7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""91372de7-4d6c-4628-b096-69ffe3c380e9"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClearSave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // General
            m_General = asset.FindActionMap("General", throwIfNotFound: true);
            m_General_Exit = m_General.FindAction("Exit", throwIfNotFound: true);
            m_General_BookToggle = m_General.FindAction("BookToggle", throwIfNotFound: true);
            m_General_BookNavigate = m_General.FindAction("BookNavigate", throwIfNotFound: true);
            // Debug
            m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
            m_Debug_ClearSave = m_Debug.FindAction("ClearSave", throwIfNotFound: true);
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

        // General
        private readonly InputActionMap m_General;
        private IGeneralActions m_GeneralActionsCallbackInterface;
        private readonly InputAction m_General_Exit;
        private readonly InputAction m_General_BookToggle;
        private readonly InputAction m_General_BookNavigate;
        public struct GeneralActions
        {
            private @Controls m_Wrapper;
            public GeneralActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Exit => m_Wrapper.m_General_Exit;
            public InputAction @BookToggle => m_Wrapper.m_General_BookToggle;
            public InputAction @BookNavigate => m_Wrapper.m_General_BookNavigate;
            public InputActionMap Get() { return m_Wrapper.m_General; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
            public void SetCallbacks(IGeneralActions instance)
            {
                if (m_Wrapper.m_GeneralActionsCallbackInterface != null)
                {
                    @Exit.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnExit;
                    @Exit.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnExit;
                    @Exit.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnExit;
                    @BookToggle.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookToggle;
                    @BookToggle.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookToggle;
                    @BookToggle.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookToggle;
                    @BookNavigate.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookNavigate;
                    @BookNavigate.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookNavigate;
                    @BookNavigate.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnBookNavigate;
                }
                m_Wrapper.m_GeneralActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Exit.started += instance.OnExit;
                    @Exit.performed += instance.OnExit;
                    @Exit.canceled += instance.OnExit;
                    @BookToggle.started += instance.OnBookToggle;
                    @BookToggle.performed += instance.OnBookToggle;
                    @BookToggle.canceled += instance.OnBookToggle;
                    @BookNavigate.started += instance.OnBookNavigate;
                    @BookNavigate.performed += instance.OnBookNavigate;
                    @BookNavigate.canceled += instance.OnBookNavigate;
                }
            }
        }
        public GeneralActions @General => new GeneralActions(this);

        // Debug
        private readonly InputActionMap m_Debug;
        private IDebugActions m_DebugActionsCallbackInterface;
        private readonly InputAction m_Debug_ClearSave;
        public struct DebugActions
        {
            private @Controls m_Wrapper;
            public DebugActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @ClearSave => m_Wrapper.m_Debug_ClearSave;
            public InputActionMap Get() { return m_Wrapper.m_Debug; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
            public void SetCallbacks(IDebugActions instance)
            {
                if (m_Wrapper.m_DebugActionsCallbackInterface != null)
                {
                    @ClearSave.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnClearSave;
                    @ClearSave.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnClearSave;
                    @ClearSave.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnClearSave;
                }
                m_Wrapper.m_DebugActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @ClearSave.started += instance.OnClearSave;
                    @ClearSave.performed += instance.OnClearSave;
                    @ClearSave.canceled += instance.OnClearSave;
                }
            }
        }
        public DebugActions @Debug => new DebugActions(this);
        public interface IGeneralActions
        {
            void OnExit(InputAction.CallbackContext context);
            void OnBookToggle(InputAction.CallbackContext context);
            void OnBookNavigate(InputAction.CallbackContext context);
        }
        public interface IDebugActions
        {
            void OnClearSave(InputAction.CallbackContext context);
        }
    }
}
