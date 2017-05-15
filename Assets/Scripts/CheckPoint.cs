using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    GameControllerScript gScript;
    cinematicAspect cinema;
    PlayerMovement playerMov;
    public bool usedForCinematicPurposes;
    public Transform moveCamera;
    public float cameraMoveSpeedToB;
    public float cameraMoveSpeedBackToA;
    public float cameraStayTimeInTarget;
    delegate void cameraAction();
    cameraAction camAction;
    Vector3 originPosition;
    bool flag = false;
    bool flag2 = false;
    float timer = 0;
    float buttonTime = 1;
    //not in use
    void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        cinema = GameObject.FindGameObjectWithTag("GameController").GetComponent<cinematicAspect>();
        playerMov = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        originPosition = this.transform.position;
    }

    private void Update()
    {    
        if (camAction != null)
        {
            camAction();
            if (Input.GetButton("Jump"))
            {
                timer += Time.unscaledDeltaTime;
            }
            else
            {
                timer = 0;
            }
            if (timer >= buttonTime)
            {
                camAction = setCinematicOff;
            }
        } 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !flag)
        {
            flag = true;
            gScript.setCheckpoint(this.transform.position);
            if (usedForCinematicPurposes)
            {
                originPosition = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position;
                camAction += moveCameraToPoint;
            }
            cinema.startCinema();
        }
    }

    void moveCameraToPoint()
    {
        if (playerMov.isGrounded())
        {
            playerMov.setPerformAction(true);
            playerMov.playAnimation("Crouch");
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, cameraMoveSpeedToB * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, 0, cameraMoveSpeedToB * Time.unscaledDeltaTime);
        }
        takeControlOfCameraAndTimeHandler(true);
        CameraScript camScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 targetPos = moveCamera.position;
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, cameraMoveSpeedToB * Time.unscaledDeltaTime);
        if (Vector3.Distance(cam.transform.position, targetPos) < 15&&!flag2)
        {
            flag2 = true;
            StartCoroutine(cameraWait());
        }
    }

    void returnToOrigin()
    {
        playerMov.playAnimation("Crouch");
        CameraScript camScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 targetPos = originPosition;
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, cameraMoveSpeedBackToA * Time.unscaledDeltaTime);
        if (Vector3.Distance(cam.transform.position, targetPos) < 10)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, cameraMoveSpeedBackToA * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, 0.2f, cameraMoveSpeedBackToA * Time.unscaledDeltaTime);
            setCinematicOff();
        }
    }
    
    void takeControlOfCameraAndTimeHandler(bool option)
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().usedSomeWhereElse(option);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<TimeHandler>().setTimeFromOutside(option);
    }

    IEnumerator cameraWait()
    {
        yield return new WaitForSecondsRealtime(cameraStayTimeInTarget);
        camAction = returnToOrigin;
        takeControlOfCameraAndTimeHandler(false);
    }

    void setCinematicOff()
    {
        playerMov.setPerformAction(false);
        takeControlOfCameraAndTimeHandler(false);
        camAction = null;
    }
}
