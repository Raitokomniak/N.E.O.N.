using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAISensing : MonoBehaviour {

    // Use this for initialization
    public AudioSource guardAudio;
    public SpriteRenderer exclamationMarkSprite;
    //public AudioClip detectionSound;
    //public AudioClip alertSound;
    public Transform eyes;
    public float enemyFieldOfView = 110f;
    public float detectionTime = 1f;
    EnemyPatrollingMovement moving;
    CircleCollider2D circle;
    BoxCollider2D box;
    Vector2 playerIsAt;
    GameObject player;
    GameControllerScript gScript;
    
    float timer;
    float anotherTimer;
    float originalVolume;
    bool playerSeen;
    bool gotHit;

    FMOD.Studio.EventInstance detectionSound;
    FMOD.Studio.EventInstance alertSound;
    FMOD.Studio.PLAYBACK_STATE detectionSoundState;

    void Awake()
    {
        moving = GetComponent<EnemyPatrollingMovement>();
        circle = GetComponent<CircleCollider2D>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        playerSeen = false;
        exclamationMarkSprite.enabled = false;
        gotHit = false;
        timer = 0;
        anotherTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        box = player.GetComponent<BoxCollider2D>();
        originalVolume = guardAudio.volume;
        // guardAudio = GetComponentInChildren<AudioSource>();
        detectionSound = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy sounds/Guard (cyborg grunt)/Alert state");
        alertSound = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Alert sound");
    }

    void Start () {
        
    }
    void Update()
    {
        if (!gotHit)
        {
            if (playerSeen)
            {
                gScript.setAlertState(true);
            }
            /*if (guardAudio.clip == alertSound && guardAudio.isPlaying)
            {
                guardAudio.volume = Mathf.Lerp(guardAudio.volume, 0.1f, 4 * Time.deltaTime);
            }*/
        }

    }
    public void setHitStatus(bool status)
    {
        gotHit = status;
        GetComponentInChildren<EnemyAIAiming>().setHitStatus(status);
        GetComponentInChildren<SensesIndicator>().setHitStatus(status);
    }
    bool detectionHandler(bool seen)
    {
        timer += Time.deltaTime;
        if (timer >= detectionTime)
        {
            //guardAudio.clip = alertSound;
            //guardAudio.volume = originalVolume;
            if (!seen)
            {
                alertSound.start();
            }
            seen = true;
            StartCoroutine(alert());
        }
        return seen;
    }
	
    void OnTriggerStay2D(Collider2D col)
    {
        if (!gScript.isDead()&&!gotHit)
        {
            seeing(col);
            if (!playerSeen && col.gameObject == player)
            {
                hearing(col);
            }
        }
        else
        {
            playerSeen = false;
            timer = 0;
            anotherTimer = 0;
        }
    }
    public void setPlayerIsAt(Vector2 place)
    {
        playerIsAt = place;
    }

    void hearing(Collider2D col)
    {
        if (player.GetComponent<PlayerMovement>().getState()== "run")
        {
            playerIsAt = col.transform.position;
            moving.playerIsHeard(playerIsAt);            
        }
    }

    void seeing(Collider2D col)
    {
        int dir = 1;
        if (!moving.facingRight())
        {
            dir *= -1;
        }

        if (col.gameObject == player)
        {
            Vector2 direction = col.transform.position - eyes.position;
            float angle = Vector2.Angle(direction, eyes.right * dir);
            if (angle < enemyFieldOfView * 0.5f)
            {
                RaycastHit2D see = Physics2D.Raycast(eyes.position, direction);
                if (see)
                {
                    if (see.collider.gameObject == player)
                    {
                        if (!playerSeen)
                        {
                            detectionSound.getPlaybackState(out detectionSoundState);
                            if(detectionSoundState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                            {
                                detectionSound.start();
                            }
                            playerSeen = detectionHandler(playerSeen);
                        }
                        else
                        {
                            playerSeen = true;
                        }
                        playerIsAt = new Vector2(box.transform.position.x, box.transform.position.y) + box.offset;
                    }
                    else
                    {
                        
                        if (playerSeen)
                        {
                            if (!checkIfPlayerIsBehind())
                            {
                                playerSeen = false;
                            }
                        }
                        else
                        {
                            playerSeen = false;
                            anotherTimer = 0;
                        }
                        timer = (playerSeen) ? 0 : timer;
                    }
                }
            }
        }
        
    }
    
    public bool checkIfPlayerIsBehind()
    {
        int dir = moving.facingRight() ? 1 : -1;
        Vector2 directionToTarget = transform.position - player.transform.position;
        float angle = Vector2.Angle(transform.right * dir, directionToTarget);
        return (!(angle >= 90)) ? true : false;
    }

    IEnumerator detect()
    {
        yield return new WaitForSeconds(0.5f);
        playerSeen = true;
    }

    IEnumerator alert()
    {
        exclamationMarkSprite.enabled = true;
        yield return new WaitForSeconds(2f);
        exclamationMarkSprite.enabled = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!gScript.isDead())
        {
            if (col.gameObject == player)
            {
                playerSeen = false;
            }
        }
    }

    IEnumerator checkPlayer()
    {
        yield return new WaitForSeconds(1f);
        playerSeen = false;
    }

    public void setPlayerInSight(bool option)
    {
        playerSeen = option;
        StartCoroutine(checkPlayer());
    }

    public bool playerInSight()
    {
        return playerSeen;
    }

    public Vector2 playerLastSeenPosition()
    {
        return playerIsAt;
    }

}
