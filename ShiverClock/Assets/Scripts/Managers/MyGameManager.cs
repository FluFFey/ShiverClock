using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
    public GameObject[] sortedLifeRemainderTexts;
    public GameObject[] spawnPositions;
    public AnimatorController[] sortedAnimatorControllers;
    public float roundDuration; //time a round takes
    private float remainingTime; //in seconds
    public GameObject HUDClock;
    public GameObject winnerText;
    public GameObject rechargeEnergyObj;
    private Timer energyRechargeTimer;
    public float energyRechargeTime;
    public float timeBeforeFirstBattery;

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
        //DontDestroyOnLoad(this);
        energyRechargeTimer = new Timer(timeBeforeFirstBattery);
    }

    // Use this for initialization
    void Start()
    {
        remainingTime = roundDuration;
        players = new GameObject[numberOfPlayers];
        Vector3 startPosPlayer1 = new Vector3(0, 0, 0);
        Vector3 posIncrement = new Vector3(2.0f, 0, 0);
        int spawnPos = Random.Range(0, spawnPositions.Length);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i] = Instantiate(playerObj, spawnPositions[(spawnPos + i) % spawnPositions.Length].transform.position, Quaternion.Euler(Vector3.zero));
            players[i].GetComponent<InputHandler>().playerID = (InputHandler.PlayerID)i;
            players[i].GetComponent<InputHandler>().setEnergySlider(sortedEnergyBars[i]);
            players[i].GetComponent<InputHandler>().setLifeCounterText(sortedLifeRemainderTexts[i]);
            players[i].GetComponent<Animator>().runtimeAnimatorController = sortedAnimatorControllers[i];
        }
        StartCoroutine(Camera.main.GetComponent<CameraScript>().fade(true, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {

        remainingTime -= Time.deltaTime * timeScale;
        int minutes = (int)remainingTime / 60;
        int seconds = (int)(remainingTime % 60);
        string timeString = minutes.ToString() + " : " + seconds.ToString();
        HUDClock.GetComponent<UnityEngine.UI.Text>().text = timeString;
        if (energyRechargeTimer.hasEnded())
        {
            energyRechargeTimer.setDuration(energyRechargeTime);
            energyRechargeTimer.restart();
            GameObject rechargeObj = Instantiate(rechargeEnergyObj);
            rechargeObj.transform.position = spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position;
        }
    }

    public void setVictor(int playerNo)
    {
        winnerText.GetComponent<Text>().text = "Player " + playerNo.ToString() + "won!";
        StartCoroutine(restartGame());
    }
    IEnumerator restartGame()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("Start");
    }

    static public GameObject[] getPlayers()
    {
        return players;
    }
    public Vector2 getSpawnPos()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position;
    }
}
