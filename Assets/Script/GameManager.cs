using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public enum EGameState
    {
        Menu,
        Starting,
        InProgress,
        PostGame
    }

    #region Properties

    [Header("SpawnSettings")]

    /* List of Spawn Points for Enemy AI */
    public List<Transform> EnemySpawnPoints;

    /* Player Spawn Point */
    public Transform PlayerSpawnPoint;

    /* Group Of Fruits */
    public Transform Fruits;
    [Header("SpawnPool")]
    /* List of AIs */
    public List<GameObject> EnemyAI;

    /* Player Reference */
    public GameObject Player;

    public static GameManager instance = null;

    /* Total Score of Player */
    public int TotalScore { get; private set; }

    /* Number of Fruits Collected by Player in this Level */ 
    public int NumFruitsCollected { get; private set; }

    /* Max Number of the Fruits Exist in this Level */
    public int MaxNumFruits { get; private set; }

    /* Game State */
    public EGameState GameState { get; private set; }

    /* Volume of Sound Effects */
    public float VolSfx = 1.0f;

    /* Volume of Background Music */
    public float VolBgm = 1.0f;

    /* Next Percentage to Relase an AI */
    private float NextAIPercentage = 20.0f;

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
        MaxNumFruits = 2;
        GameState = EGameState.Menu;

        //Load Setting
        LoadSetting();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    #region Save/Load

    //Save Settings to Player Prefs
    public void SaveSetting()
    {
        //Save Settings
        PlayerPrefs.SetFloat("S_SFX_V", VolSfx);
        PlayerPrefs.SetFloat("S_BGM_V", VolBgm);
        PlayerPrefs.Save();
    }

    //Load Setting from Player Prefs
    public void LoadSetting()
    {
        VolBgm = PlayerPrefs.GetFloat("S_BGM_V", 0.5f);
        VolSfx = PlayerPrefs.GetFloat("S_SFX_V", 0.5f);
        Utility.UtilInstance.UpdateSFXVolume();
    }

    #endregion



    #region PreGame Handling
    public void TryStartGame()
    {
        //Set Game State
        GameState = EGameState.InProgress;

        //Reset Player Camera Rotation
        Camera.main.transform.eulerAngles = new Vector3(65.0f, 0.0f, 0.0f);

        //Change Audio to In Game Music
        if (Utility.UtilInstance != null)
            Utility.UtilInstance.PlayMusic(Utility.UtilInstance.BGM_Game);


        //Reset Scores and Properties
        TotalScore = 0;
        NumFruitsCollected = 0;


        //UI Manager
        if (UIManager.UIInstance != null)
        {

            //Hide Main Menu
            UIManager.UIInstance.SetMenuActive(false);

            //Show Game UI
            UIManager.UIInstance.SetGameUIActive(true);

            //Hide Post Game UI in case Of Reset
            UIManager.UIInstance.SetPostGameUIActive(false);

            //Reset Text Score
            UIManager.UIInstance.UpdateScoreText();
        }

        //Reset Game Object
        ResetPlayer();
        ResetEnemyAI();
        ResetMap();
    }

    // Reset Player Position , Velocity , Active Flag
    private void ResetPlayer()
    {
        if(Player != null)
        {
            //Stop Player Velocity
            Player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            //Reset Player Position
            Player.transform.position = PlayerSpawnPoint.position;

            //Enable Player
            Player.SetActive(true);

        }
    }

    //Reset AI Enemies back to their Location and State
    private void ResetEnemyAI()
    {
        for(int i = 0; i < EnemyAI.Count; i++)
        {
            EnemyAIBase AIBrain = EnemyAI[i].GetComponent<EnemyAIBase>();

            

            //Set Their Position
            if (EnemySpawnPoints.Count > i)
            {
                EnemyAI[i].transform.position = EnemySpawnPoints[i].position;
            }

            //Default Active AIs
            if (i < 2)
            {
                //Activate Class/Brain
                AIBrain.enabled = true;

                //Set Their State
                AIBrain.ResetAI();
            }
            else
            {
                //Ghost AIs
                EnemyAI[i].GetComponent<NavMeshAgent>().enabled = false;
                AIBrain.enabled = false;

            }



        }
    }

    //Reset Map Fruits to Pick
    private void ResetMap()
    {

        int NumChild = Fruits.childCount;
        for (int i = 0; i < NumChild; i++)
        {
            Fruits.GetChild(i).gameObject.SetActive(true);
        }

        MaxNumFruits = NumChild;
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

        //Update Score Text
        if (UIManager.UIInstance != null)
            UIManager.UIInstance.UpdateScoreText();
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
        AddScore(10);


        //Check if All the Fruits are Collected then Win the Game
        if (NumFruitsCollected >= MaxNumFruits)
            LevelCompleted();
        else
        {
            //If Player is progressing Rlease another AI
            float Percentage = (NumFruitsCollected * 100) / MaxNumFruits;
            if (Percentage > NextAIPercentage)
                ReleaseAI();
                
        }
       

    }

    private void ReleaseAI()
    {
        
        for(int i = 0; i < EnemyAI.Count; i++)
        {
            EnemyAIBase AIBrain = EnemyAI[i].GetComponent<EnemyAIBase>();

            if (!AIBrain.enabled)
            {
                EnemyAI[i].GetComponent<NavMeshAgent>().enabled = true;
                AIBrain.enabled = true;
                AIBrain.ResetAI();
                NextAIPercentage += 25f;
                return;
            }
        }
    }

    //Called by Player to Notify Death
    public void OnPlayerDeath()
    {
        //Disable AI, Player

        //Show End Game UI
        if (UIManager.UIInstance != null)
        {
            UIManager.UIInstance.SetGameUIActive(false);
            UIManager.UIInstance.SetPostGameUIActive(true);
            UIManager.UIInstance.InitlizePostGameResult("GameOver!",
                TotalScore,
                NumFruitsCollected);
        }
    } 
    #endregion

    #region PostGame Handling

    // Called to Notify Level Completion Conditions are met
    private void LevelCompleted()
    {
        GameState = EGameState.PostGame;


        //Disable AI, Player

        //Show End Game UI
        if (UIManager.UIInstance != null)
        {
            UIManager.UIInstance.SetGameUIActive(false);
            UIManager.UIInstance.SetPostGameUIActive(true);
            UIManager.UIInstance.InitlizePostGameResult("You Won!",
                TotalScore,
                NumFruitsCollected);
        }
            
    }

    #endregion
}
