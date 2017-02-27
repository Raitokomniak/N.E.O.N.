using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

    public float x1;               //Most left point
    public float x2;                //Most right point
    public int StepsOnSecond = 30;      //how much object moves every frame
    public bool Flag = true;            //set false to start object from up position. true to start from bottom.

    private float x;
    private float xDelta;
    private float y;

    void Start()
    {
        xDelta = x2 - x1;
        y = transform.position.y;
        if (Flag)
        {
            transform.position = new Vector2(x1, y);
        }
        else
        {
            transform.position = new Vector2(x2, y);
        }
    }

    void Update()
    {
        if (Flag == true)
        {
            if (transform.position.x >= x2)
            {
                Flag = false;
            }
            else
            {

                moveRight();
            }
        }

        else if (Flag == false)
        {
            if (transform.position.x <= x1)
            {
                Flag = true;
            }
            else
            {
                moveLeft();
            }

        }
    }


    private void moveRight()
    {
        x = transform.position.x + (xDelta / StepsOnSecond * (Time.deltaTime * 5));
        transform.position = new Vector2(x, y);
    }

    private void moveLeft()
    {
        x = transform.position.x - (xDelta / StepsOnSecond * (Time.deltaTime * 5));
        transform.position = new Vector2(x, y);
    }
}
