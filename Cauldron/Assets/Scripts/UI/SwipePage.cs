using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
public class SwipePage : MonoBehaviour
{
    [Header("Листание слайдом")]
    [SerializeField] RectTransform rightPage;
    [SerializeField] float minTouchTime = 0.15f;
    [SerializeField] bool toNext;
    RecipeBook recipeBook; 
    Vector3[] rightPageCorners = new Vector3[4];
    Vector2 minPositionPage, maxPositionPage;
    Vector2 mousePosition, prevMousePosition;
    float touchTime = 0f;
    bool isHolding = false;
    

    void Start()
    {       
        recipeBook = RecipeBook.instance;
        rightPage.GetWorldCorners(rightPageCorners); // 0 и  2 хранят минимальные и максимальные x,y для страницы
        minPositionPage = rightPageCorners[0];
        maxPositionPage = rightPageCorners[2];
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
            {
                StartCount();
                touchTime += Time.deltaTime;
            }

        if (Input.GetMouseButtonUp(0))
        {
            if ((isHolding) && (touchTime > minTouchTime)) // если была зажата кнопка и прошло время
            {
                if ((toNext) && (prevMousePosition.x > Input.mousePosition.x))
                    { recipeBook.NextPage();}
                else if ((!toNext) && (prevMousePosition.x < Input.mousePosition.x))
                    { recipeBook.PrevPage();}    
            }
            touchTime = 0f;
            isHolding = false;
            prevMousePosition = Vector2.negativeInfinity; // не 0, т.к. это может быть координатой
        }           
    }

    void StartCount()
    {
    if ((!isHolding) && IsMouseInside())
        {
            prevMousePosition = Input.mousePosition;
            isHolding = true;
        }
    }

    bool IsMouseInside()
    {
        mousePosition = Input.mousePosition;
        if ((mousePosition.x >= minPositionPage.x) && (mousePosition.y >= minPositionPage.y) 
                    && (mousePosition.x <= maxPositionPage.x) && (mousePosition.y <= maxPositionPage.y))
            { return true;}
        else
            { return false;}
    } 
}
}
