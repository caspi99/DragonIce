using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersController : MonoBehaviour
{
    //<-------------------PLAYERS INDEXES------------------------>
    const int PLAYER_1 = 0;
    const int PLAYER_2 = 1;
    const int PLAYER_3 = 2;
    const int PLAYER_4 = 3;

    //<------------------------PLAYERS--------------------------->
    [SerializeField]
    protected List<GameObject> Players;

    //<---------------------TIME VARIABLES------------------------->
    protected int timer = -1;
    protected float update_data_time;

    //<---------------------EXTERNAL CONTROLLERS------------------------->
    protected TimeManager tm;
    protected DataExtractorCSV data_extractor;

    // Start is called before the first frame update
    protected void Start()
    {
        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSV>();

        InitTimer();

        update_data_time = Settings.update_data_time;

        if (SceneManager.GetActiveScene().name.Equals("Level1"))
        {
            ChangePlayerVisibility(Settings.visualizePlayerPosition);
        }
        else
        {
            ChangePlayerVisibility(Settings.visualizePlayerPosition);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (WaitTime(update_data_time))
        {
            for (int i = 0; i < 4; i++)
            {
                (float x, float y) = GetPositionOfPlayer(i);

                data_extractor.WriteDataLinePositionPlayer(i, x, y);
            }
        }
    }

    //<----------------------METHODS THAT USES THE PLAYERS CONTROLLER-------------------------->
    protected (float, float) GetPositionOfPlayer(int idx)
    {
        return (Players[idx].transform.position.x, Players[idx].transform.position.z);
    }

    //<----------------------METHODS TO INITIALIZE THE PLAYERS CONTROLLER-------------------------->

    public void ChangePlayerVisibility(bool status)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].transform.Find("Cube").GetComponent<MeshRenderer>().enabled = status; //ocultar player
        }
    }
    protected void ChangePlayersColliderStatus(bool status)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<CapsuleCollider>().enabled = status;
        }
    }

    public void EnablePlayersCollider() { ChangePlayersColliderStatus(true); }
    public void DisablePlayersCollider() { ChangePlayersColliderStatus(false); }

    //<----------------------TIMERS METHODS-------------------------->

    //method to init the timer
    private void InitTimer()
    {
        if (timer < 0) { timer = tm.CreateTimer(); tm.PauseTimer(timer); }
    }

    //method to count the seconds of the stones without moving
    private bool WaitTime(float time) { return tm.WaitTimeWithReset(timer, time); }
}
