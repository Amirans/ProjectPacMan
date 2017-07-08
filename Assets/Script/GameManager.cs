using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum EGameState
    {
        Menu,
        Starting,
        InProgress,
        PostGame
    }

    #region Properties

    public static GameManager instance = null;

    /* Total Score of Player */
    public int TotalScore { get; private set; }

    /* Number of Fruits Collected by Player in this Level */ 
    public int NumFruitsCollected { get; private set; }

    /* Max Number of the Fruits Exist in this Level */
    public int MaxNumFruits { get; private set; }

    /* Game State */
    public EGameState GameState { get; private set; }

    #endregion

    //Use this for Pre-Start Initialization
    void Awake()
    {

        //Check if the Instance of Game Manager Exist Else Create One
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(instance);

        //Flag this Object to Not be Removed when Loading or Restarting
        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start () {
        MaxNumFruits = 1;
        GameState = EGameState.InProgress;
	}
	
	// Update is called once per frame
	void Update ()
    {
	}



    #region PreGame Handling
    public void TryStartGame()
    {
        //Set Game State
        GameState = EGameState.Starting;

        //TODO: Hide Main Menu
        //TODO: Reset Game Object

        //Reset Scores and Properties
        TotalScore = 0;
        NumFruitsCollected = 0;

        //TODO: Call to Start the Game
    }
    #endregion

    #region GameInProgress Handling
    //Adds Score to the Game Total Score
    public void AddScore(int ScoreValue)
    {
        //Exit If Game is not in Progress
        if (GameState != EGameState.InProgress)
            return;

        TotalScore += ScoreValue;
        Debug.Log(ScoreValue + " Score Added, Totaling in: " + TotalScore);
    }

    //Called by Player to Notify Fruits is Collected
    public void OnFruitCollected()
    {
        //Exit If Game is not in Progress
        if (GameState != EGameState.InProgress)
            return;

        //increment Fruits Number 
        NumFruitsCollected += 1;

        //Add Score for Collecting
        AddScore(1);

        //Check if All the Fruits are Collected then Win the Game
        if (NumFruitsCollected >= MaxNumFruits)
            LevelCompleted();

    }
    #endregion

    #region PostGame Handling

    // Called to Notify Level Completion Conditions are met
    private void LevelCompleted()
    {
        GameState = EGameState.PostGame;

        
        Debug.Log("Won!");
        //Todo: Show End Game UI
    }

    #endregion
}
