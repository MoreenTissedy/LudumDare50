using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class CheatCodes
{
    public string CheatCode;
    public UnityEvent<string> Event;
}

public class InputCheatManager : MonoBehaviour
{
    [SerializeField]
    private string compositeString;

    [SerializeField,
    Range(.1f, 2f)]
    private float clearAfter = .75f;

    [SerializeField]
    private List<CheatCodes> cheatCodes;

    private InputAction _anyKeyWait;
    private Coroutine _clearTimer;
    private float _timeUntilClear;


    private void Awake()
    {
        _anyKeyWait = new InputAction(type:InputActionType.Button);
    }

    private void OnEnable()
    {
        Keyboard.current.onTextInput += InputKey;
        _anyKeyWait.Enable();
        _clearTimer = StartCoroutine(ClearTimer());
    }

    private void OnDisable()
    {
        StopCoroutine(_clearTimer);
        Keyboard.current.onTextInput -= InputKey;
        _anyKeyWait.Disable();
    }

    private void InputKey(char inputChar)
    {
        compositeString += inputChar.ToString().ToLower();
        _timeUntilClear = 0;
        CheckCodes();
    }

    private void CheckCodes()
    {
        foreach(var cheat in cheatCodes)
        {
            if (compositeString.Equals(cheat.CheatCode))
                cheat.Event.Invoke(compositeString);
        }
    }
   
    private IEnumerator ClearTimer()
    {
        while (true)
        {
            if (compositeString == string.Empty)
                yield return null;

            _timeUntilClear += Time.deltaTime;

            if (_timeUntilClear >= clearAfter)
            {
                _timeUntilClear -= clearAfter;
                compositeString = string.Empty;
            }
            yield return null;
        }
    }
}