using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum EState
    {
        Play,
        Pause,
        Wasted,
        GameOver
    }
    private EState eState;

    private int[] playerLife = new int[] { 3, 3 };
    private int[] playerScore = new int[] { 0, 0 };
    public GameObject Player2;

    public GameObject Player;
    public GameObject GUIManager;

    private AudioSource Audio;
    public AudioClip Wasted;

    public List<GameObject> aCarGenerators;

    // Objects spawning
    public List<BoxCollider2D> aCollision;
    public GameObject SpawnPointsStartLocation;
    public GameObject ObjectivePrefab;
    private List<Vector3> aSpawnList;

    private float fTimerDt;

    void Start()
    {
        if (StaticDatas.b2Player)
        {
            Player2.SetActive(true);
            GUIManager.GetComponent<GUIManager>().DisplayPlayer2Infos(playerLife[1], playerScore[1]);
        }

        Audio = GetComponent<AudioSource>();
        eState = EState.Play;

        aSpawnList = new List<Vector3>();

        Vector3 vStartLoc = SpawnPointsStartLocation.transform.position;
        Vector3 vSpawnLoc = vStartLoc;
        while (vSpawnLoc.y >= vStartLoc.y * -1)
        {
            while (vSpawnLoc.x <= vStartLoc.x * -1)
            {
                bool bAdd = true;
                foreach(var collider in aCollision)
                {
                    if (collider.bounds.Contains(vSpawnLoc))
                    {
                        bAdd = false;
                        break;
                    }
                }

                if (bAdd)
                {
                    aSpawnList.Insert(Random.Range(0, aSpawnList.Count), vSpawnLoc);
                    
                }

                vSpawnLoc.x += 16.0f;
            }
            vSpawnLoc.x = vStartLoc.x;
            vSpawnLoc.y -= 16.0f;
        }

        SpawnObjective();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        bool bPause = Input.GetKeyDown(KeyCode.P);

        switch (eState)
        {
            case EState.Pause:
                {
                    if (bPause)
                    {
                        Time.timeScale = 1.0f;
                        Time.fixedDeltaTime = 0.02f;
                        eState = EState.Play;

                        GUIManager.GetComponent<GUIManager>().DisplayBackground(false);
                        GUIManager.GetComponent<GUIManager>().DisplayPauseText(false);
                    }
                }
                break;

            case EState.Play:
                {
                    if (bPause)
                    {
                        Time.timeScale = 0.0f;
                        Time.fixedDeltaTime = 0.0f;
                        eState = EState.Pause;

                        GUIManager.GetComponent<GUIManager>().DisplayBackground(true);
                        GUIManager.GetComponent<GUIManager>().DisplayPauseText(true);
                    }
                }
                break;

            case EState.Wasted:
                {
                    fTimerDt += Time.deltaTime;
                    if (fTimerDt >= 2.2f*0.4f)
                    {
                        Time.timeScale = 0.1f;
                        Time.fixedDeltaTime = 0.02f * Time.timeScale;

                        GUIManager.GetComponent<GUIManager>().DisplayWastedText();

                        if (fTimerDt >= 4.6f * 0.1f)
                        {
                            Time.timeScale = 0.0f;
                            Time.fixedDeltaTime = 0.0f;

                            GUIManager.GetComponent<GUIManager>().DisplayReplayText();

                            eState = EState.GameOver;
                        }
                    }
                }
                break;

            case EState.GameOver:
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        Time.timeScale = 1.0f;
                        Time.fixedDeltaTime = 0.02f;
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                break;
        }
    }

    public bool OnPlayerHit(int playerId)
    {
        playerLife[playerId - 1]--;

        GUIManager.GetComponent<GUIManager>().DisplayPlayer1Infos(playerLife[0], playerScore[0]);
        if (StaticDatas.b2Player)
        {
            GUIManager.GetComponent<GUIManager>().DisplayPlayer2Infos(playerLife[1], playerScore[1]);
        }

        if (playerLife[playerId - 1] == 0)
        {
            eState = EState.Wasted;
            Audio.clip = Wasted;
            Audio.loop = false;
            Audio.Play();

            Time.timeScale = 0.4f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            GUIManager.GetComponent<GUIManager>().DisplayBackground(true);

            return false;
        }

        return eState == EState.Play;
    }

    private void SpawnObjective()
    {
        Instantiate(ObjectivePrefab, aSpawnList[Random.Range(0, aSpawnList.Count)], Quaternion.identity);
    }

    public void OnPlayerTookObjective(int playerId)
    {
        playerScore[playerId - 1]++;

        GUIManager.GetComponent<GUIManager>().DisplayPlayer1Infos(playerLife[0], playerScore[0]);
        if (StaticDatas.b2Player)
        {
            GUIManager.GetComponent<GUIManager>().DisplayPlayer2Infos(playerLife[1], playerScore[1]);
        }

        SpawnObjective();

        foreach (var gen in aCarGenerators)
        {
            gen.GetComponent<CarGenerator>().fCarSpeed += 100.0f;
        }
    }
}
