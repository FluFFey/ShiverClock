using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    [Range(0,10)]
    public float timeScale;
    public static MyGameManager instance;
    public GameObject playerObj;
    public float defaultGravity;
    private static GameObject[] players;
    [Range(2, 4)]
    public int numberOfPlayers;
    public GameObject[] sortedEnergyBars;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        players = new GameObject[numberOfPlayers];
        Vector3 startPosPlayer1 = new Vector3(0, 0, 0);
        Vector3 posIncrement = new Vector3(2.0f, 0, 0);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i] = Instantiate(playerObj, startPosPlayer1 + posIncrement * i, Quaternion.Euler(Vector3.zero));
            players[i].GetComponent<InputHandler>().playerID = (InputHandler.PlayerID)i;
            players[i].GetComponent<InputHandler>().setEnergySlider(sortedEnergyBars[i]);
        }
        StartCoroutine(Camera.main.GetComponent<CameraScript>().fade(true, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = timeScale;
    }

    static public GameObject[] getPlayers()
    {
        return players;
    }
}
