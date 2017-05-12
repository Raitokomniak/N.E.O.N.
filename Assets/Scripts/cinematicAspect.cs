using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class cinematicAspect : MonoBehaviour {
    public CanvasRenderer blackScreen;
    public Text titleText;
    public string textForTitle;
    public GameObject upBar;
    public GameObject downBar;
    public Camera mainCamera;
    TimeHandler timeHandler;
    public float divider;
    public int holdTime = 3;
    public bool showBlackScreen;
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
    bool flag;
    // Use this for initialization

    private void Awake()
    {
        flag = true;
        timeHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<TimeHandler>();
    }

    
    void Start ()
    {
        deltaUpY = up1Y - up2Y;
        deltaDownY = down2Y - down1Y;
        deltaFieldOfView = cinematicFieldOfView - startingFieldOfView;
    }
	
	// Update is called once per frame
    public void setTitleScreen(string text)
    {
        textForTitle = text;
        flag = true;
        showBlackScreen = true;
    }

    IEnumerator waitBlack()
    {
        yield return new WaitForSecondsRealtime(2f);
        showBlackScreen = false;
    }
	void Update ()
    {
        if (showBlackScreen&&flag)
        {
            if (!GetComponent<GameControllerScript>().isDead())
            {
                setScreen(1);
                timeHandler.setTimeFromOutside(true);
                Time.timeScale = 0;
                Time.fixedDeltaTime = 0;
                titleText.enabled = true;
                titleText.text = textForTitle;
                StartCoroutine(waitBlack());
                flag = false;
            }
        }

        if (!showBlackScreen && !flag)
        {
            frasierize();
            if (blackScreen.GetAlpha() < 0.6f)
            {
                timeHandler.setTimeFromOutside(false);
            }
            titleText.color = new Vector4(titleText.color.r, titleText.color.g, titleText.color.b, blackScreen.GetAlpha());
        }
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
                upY = upBar.transform.localPosition.y - (deltaUpY / divider * Time.unscaledDeltaTime);
                upBar.transform.localPosition = new Vector3(upBar.transform.localPosition.x, upY, upBar.transform.localPosition.z);
                downY = downBar.transform.localPosition.y + (deltaDownY / divider * Time.unscaledDeltaTime);
                downBar.transform.localPosition = new Vector3(downBar.transform.localPosition.x, downY, downBar.transform.localPosition.z);
                mainCamera.fieldOfView = mainCamera.fieldOfView + (deltaFieldOfView / divider * Time.unscaledDeltaTime);
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
                upY = upBar.transform.localPosition.y + (deltaUpY / divider * Time.unscaledDeltaTime);
                upBar.transform.localPosition = new Vector3(upBar.transform.localPosition.x, upY, upBar.transform.localPosition.z);
                downY = downBar.transform.localPosition.y - (deltaDownY / divider * Time.unscaledDeltaTime);
                downBar.transform.localPosition = new Vector3(downBar.transform.localPosition.x, downY, downBar.transform.localPosition.z);
                mainCamera.fieldOfView = mainCamera.fieldOfView - (deltaFieldOfView / divider * Time.unscaledDeltaTime);
            }
        }
	}

    void setScreen(int value)
    {
        blackScreen.SetAlpha(value);
    }
    void frasierize()
    {
        blackScreen.SetAlpha(Mathf.Lerp(blackScreen.GetAlpha(), 0, Time.unscaledDeltaTime));
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
