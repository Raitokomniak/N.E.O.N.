using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cinematicAspect : MonoBehaviour {
    public GameObject upBar;
    public GameObject downBar;
    public Camera mainCamera;
    public float divider;
    public int holdTime = 3;
    bool start = false;
    float up1Y = 277;
    float up2Y = 230;
    float deltaUpY;
    float down1Y = -277;
    float down2Y = -230;
    float deltaDownY;
    float startingFieldOfView = 60;
    float cinematicFieldOfView = 70;

    float deltaFieldOfView;
    float upY;
    float downY;
    bool back;
    // Use this for initialization
    void Start () {
        deltaUpY = up1Y - up2Y;
        deltaDownY = down2Y - down1Y;
        deltaFieldOfView = cinematicFieldOfView - startingFieldOfView;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (start)
        {
            //move bars into the picture
            //widen the field of view
            //wait for x seconds
            //pull back the bars and narow field of view back
            //start = false

            if (upBar.transform.localPosition.y <= up2Y)
            {
                StartCoroutine(wait());
            }
            else
            {
                upY = upBar.transform.localPosition.y - (deltaUpY / divider * Time.deltaTime);
                upBar.transform.localPosition = new Vector3(upBar.transform.localPosition.x, upY, upBar.transform.localPosition.z);
                downY = downBar.transform.localPosition.y + (deltaDownY / divider * Time.deltaTime);
                downBar.transform.localPosition = new Vector3(downBar.transform.localPosition.x, downY, downBar.transform.localPosition.z);
                mainCamera.fieldOfView = mainCamera.fieldOfView + (deltaFieldOfView / divider * Time.deltaTime);
            }
        }
        if (back)
        {
            if (upBar.transform.localPosition.y >= up1Y)
            {
                back = false;
            }
            else
            {
                upY = upBar.transform.localPosition.y + (deltaUpY / divider * Time.deltaTime);
                upBar.transform.localPosition = new Vector3(upBar.transform.localPosition.x, upY, upBar.transform.localPosition.z);
                downY = downBar.transform.localPosition.y - (deltaDownY / divider * Time.deltaTime);
                downBar.transform.localPosition = new Vector3(downBar.transform.localPosition.x, downY, downBar.transform.localPosition.z);
                mainCamera.fieldOfView = mainCamera.fieldOfView - (deltaFieldOfView / divider * Time.deltaTime);
            }
        }
	}

    public void startCinema()
    {
        start = true;
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(holdTime);
        start = false;
        back = true;
    }
}
