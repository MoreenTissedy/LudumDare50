using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeCursor : MonoBehaviour
{
    [SerializeField] Texture2D gameCursor;

    void Awake()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
    }

}
