using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor.Animations;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
    [Range(0,10)]
    public float timeScale;
    public float[] timeScalePrPlayer;
    public static MyGameManager instance;
    public GameObject playerObj;
    public float defaultGravity;
    
    private static GameObject[] players;
    [Range(2, 4)]
    public int numberOfPlayers;
    public GameObject[] sortedEnergyBars;
    public GameObject[] sortedLifeRemainderTexts;
    public GameObject[] spawnPositions;
    public RuntimeAnimatorController[] sortedAnimatorControllers;
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
        timeScalePrPlayer = new float[numberOfPlayers];
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
            timeScalePrPlayer[i] = 1.0f;
        }
        StartCoroutine(Camera.main.GetComponent<CameraScript>().fade(true, 1.0f));
        winnerText.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        timeScale = 0.0f;
        foreach (float playerTimeScale in timeScalePrPlayer)
        {
            //print(playerTimeScale);
            timeScale += playerTimeScale;
        }
        timeScale /= 2.0f;
        Vector2 averagePos = Vector2.zero;
        foreach(GameObject player in players)
        {
            averagePos += (Vector2)player.transform.position;
        }
        float zoom = Mathf.Abs((players[0].transform.position - players[1].transform.position).magnitude)*-1; 
        averagePos /= players.Length; //only works for 2 players
        Camera.main.GetComponent<CameraScript>().target = averagePos;
        Camera.main.GetComponent<CameraScript>().zoom = zoom;

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
        string playerColor ="";
        switch (playerNo)
        {
            case 0:
                playerColor = "Red";
                break;
            case 1:
                playerColor = "Blue";
                break;
        }
        winnerText.GetComponent<Text>().text = playerColor +" won!";
        StartCoroutine(upscaleWinText());
        
        StartCoroutine(restartGame());
    }
    IEnumerator upscaleWinText()
    {
        winnerText.transform.localScale = Vector3.zero;
        for (float i = 0; i < 1.5f; i+=Time.deltaTime)
        {
            winnerText.transform.localScale = Vector3.one * (i / 1.5f);
            yield return null;
        }
        Color newColor = winnerText.GetComponent<Text>().color;
        for (float i = 0; i < 4; i += Time.deltaTime)
        {
            newColor.a = (Mathf.Sin(i*3)+1.0f)*0.125f+0.75f;
            winnerText.transform.localScale = Vector3.one*(1 + (Mathf.Sin(i * 15))*0.1f);
            winnerText.GetComponent<Text>().color = newColor;
            yield return null;
        }

    }

    IEnumerator restartGame()
    {
        yield return new WaitForSeconds(6.0f);
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
