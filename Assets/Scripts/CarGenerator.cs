using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGenerator : MonoBehaviour
{
    enum EState
    {
        Idle,
        Alert,
        Spawn
    }
    private EState eState;

    public List<GameObject> aCars;

    public float fDelayMin = 1.0f; // Delay between cars generation
    public float fDelayMax = 5.0f;
    private float fDelay;
    private float fTimerDt;

    public float fCarSpeed = 2000.0f;

    public int CarsPerWaveMin = 5;
    public int CarsPerWaveMax = 10;

    private Vector3 vCarSpawn;

    //Arrow blinking
    public GameObject Arrow;
    private int nBlink = 6;
    private int blinkCounter = 0;
    private float fBlinkTime = 0.3f;

    void Start()
    {
        eState = EState.Idle;
        vCarSpawn = Vector3.Scale(new Vector3(32.0f, 32.0f, 32.0f), transform.right);

        fDelay = Random.Range(fDelayMin, fDelayMax);

        Arrow.SetActive(false);
    }

    void Update()
    {
        switch (eState)
        {
            case EState.Idle:
                {
                    fTimerDt += Time.deltaTime;
                    if (fTimerDt >= fDelay)
                    {
                        fTimerDt = 0.0f;
                        blinkCounter = 0;
                        eState = EState.Alert;
                    }
                }
                break;

            case EState.Alert:
                {
                    fTimerDt += Time.deltaTime;
                    if (fTimerDt >= fBlinkTime)
                    {
                        blinkCounter++;
                        fTimerDt = 0.0f;
                        Arrow.SetActive(blinkCounter % 2 != 0);

                        if (blinkCounter == nBlink)
                        {
                            eState = EState.Spawn;
                        }
                    }
                }
                break;

            case EState.Spawn:
                {
                    SpawnCars();

                    fTimerDt = 0.0f;
                    fDelay = Random.Range(fDelayMin, fDelayMax);
                    eState = EState.Idle;
                }
                break;
        }
    }

    void SpawnCars()
    {
        int nCars = Random.Range(CarsPerWaveMin, CarsPerWaveMax);

        for (int i = 0; i < nCars; ++i)
        {
            int carType = Random.Range(0, aCars.Count);
            GameObject car = Instantiate(aCars[carType], transform.position + (vCarSpawn * -i), Quaternion.identity);
            car.GetComponent<CarController>().Initialize(fCarSpeed, transform.right);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 48.0f);

        Gizmos.color = Color.white;
        Gizmos.DrawIcon(transform.position, "GunSpawner");
    }
}
