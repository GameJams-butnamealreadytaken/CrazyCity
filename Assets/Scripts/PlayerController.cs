using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    enum EState
    {
        Respawn,
        Alive,
        Dead
    }
    private EState eState;

    public float speed = 10.0f;
    public List<GameObject> aSprites;

    public GameObject GameManager;
    private AudioSource Audio;
    public AudioClip HitSound;
    public List<AudioClip> aDashSound;

    private Vector3 vStartLocation;
    private Vector3 vLoc;
    private Transform tr;

    private float CellSize = 16.0f;

    [Header("Inputs")]
    public KeyCode InputUp;
    public KeyCode InputLeft;
    public KeyCode InputDown;
    public KeyCode InputRight;

    // Respawn blinking
    private int nBlink = 5;
    private int blinkCounter = 0;
    private float fBlinkTime = 0.3f;
    private float fBlinkDt = 0.0f;

    public int playerId = 1;

    private Rigidbody2D rbody;

    void Start()
    {
        vLoc = transform.position;
        vStartLocation = transform.position;
        tr = transform;

        eState = EState.Respawn;

        Audio = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody2D>();
    }

    private bool IsMovementAllowed(Vector2 direction)
    {
        // first collider is player's one, more is from environnement
        RaycastHit2D[] aHits = Physics2D.RaycastAll(transform.position, direction, 20.0f);
        foreach (var Hit in aHits)
        {
            if (Hit.collider.gameObject == gameObject)
                continue;

            if (!Hit.collider.gameObject.CompareTag("Objective"))
            {
                return false;
            }
        }

        return true;
    }

    void Update()
    {
        // Nice pause handler
        if (Time.deltaTime == 0.0f)
            return;

        switch (eState)
        {
            case EState.Dead: break;

            case EState.Respawn:
                {
                    fBlinkDt += Time.deltaTime;
                    if (fBlinkDt >= fBlinkTime)
                    {
                        blinkCounter++;
                        fBlinkDt = 0.0f;
                        aSprites[2].SetActive(blinkCounter%2 != 0);

                        if (blinkCounter == nBlink)
                        {
                            eState = EState.Alive;
                        }
                    }
                }
                break;

            case EState.Alive:
                {
                    // Conditions: Key + Last movement finished + No collision with environnement

                    int update = -1;
                    if (Input.GetKeyDown(InputUp) && tr.position == vLoc && IsMovementAllowed(Vector2.up))
                    {
                        vLoc += Vector3.up * CellSize;
                        update = 0;
                    }
                    else if (Input.GetKeyDown(InputLeft) && tr.position == vLoc && IsMovementAllowed(Vector2.left))
                    {
                        vLoc += Vector3.left * CellSize;
                        update = 1;
                    }
                    else if (Input.GetKeyDown(InputDown) && tr.position == vLoc && IsMovementAllowed(Vector2.down))
                    {
                        vLoc += Vector3.down * CellSize;
                        update = 2;
                    }
                    else if (Input.GetKeyDown(InputRight) && tr.position == vLoc && IsMovementAllowed(Vector2.right))
                    {
                        vLoc += Vector3.right * CellSize;
                        update = 3;
                    }

                    if (update != -1)
                    {
                        foreach (var g in aSprites)
                        {
                            g.SetActive(false);
                        }

                        aSprites[update].SetActive(true);

                        Audio.clip = aDashSound[Random.Range(0, aDashSound.Count)];
                        Audio.Play();
                    }

                    transform.position = Vector3.MoveTowards(transform.position, vLoc, speed);
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            GameManager.GetComponent<GameManager>().OnPlayerTookObjective(playerId);
            Destroy(collision.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (EState.Alive != eState)
            return;

        if (collision.collider.gameObject.CompareTag("Car"))
        {
            bool bRespawn = GameManager.GetComponent<GameManager>().OnPlayerHit(playerId);
            eState = bRespawn ? EState.Respawn : EState.Dead;

            if (bRespawn)
            {
                Audio.clip = HitSound;
                Audio.Play();

                // Reset sprites
                foreach (var g in aSprites)
                {
                    g.SetActive(false);
                }

                // Reset blinking
                fBlinkDt = 0.0f;
                blinkCounter = 0;

                // Reset location
                transform.position = vStartLocation;
                vLoc = vStartLocation;

                rbody.velocity = new Vector2(0.0f, 0.0f);
                rbody.angularVelocity = 0.0f;
            }
        }
    }
}
