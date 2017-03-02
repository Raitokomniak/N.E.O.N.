using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevetorMovement : MonoBehaviour {

    public float y1;               //lowest point that object has when moving on y axis
    public float y2;                //highest point that object has when moving on y axis
    public int StepsOnSecond = 30;      //how much object moves every frame
    public bool Flag = true;            //set false to start object from up position. true to start from bottom.

    private float x;
    private float yDelta;
    private float y;

    void Start()
    {
        yDelta = y2 - y1;
        x = transform.position.x;
        if (Flag)
        {
            transform.position = new Vector2(x, y1);
        }
        else
        {
            transform.position = new Vector2(x, y2);
        }
    }

    void Update()
    {
        if (Flag == true)
        {
            if (transform.position.y >= y2)
            {
                Flag = false;
            }
            else
            {
                
                trapUp();
            }
        }

        else if (Flag == false)
        {
            if (transform.position.y <= y1)
            {
                Flag = true;
            }
            else
            {
                trapDown();
            }

        }
    }


    private void trapUp()
    {
        y = transform.position.y + (yDelta / StepsOnSecond * (Time.deltaTime * 5));
        transform.position = new Vector2(x, y);
    }

    private void trapDown()
    {
        y = transform.position.y - (yDelta / StepsOnSecond * (Time.deltaTime * 5));
        transform.position = new Vector2(x, y);
    }
}
