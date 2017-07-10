using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    [Header("UI References")]
    public Transform PanelMainMenu;
    public Transform PanelInGame;
    public Transform PanelPostGame;

    [Header("Setting")]
    public Transform PanelSettings;
    public Slider Slider_SFX;
    public Slider Slider_Music;

    [Header("InGame")]
    public Text Txt_Score;

    [Header("PostGame")]
    public Text Txt_PostMessage;
    public Text Txt_TotalScore;
    public Text Txt_TotalFruits;


    /* Static Global Reference to Instance of Utility Class */
    public static UIManager UIInstance;

    // Use this for initialization
    void Awake () {

        //Implementing Singleton
        if (UIInstance == null)
            UIInstance = this;
        else if (UIInstance != null)
            Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {


    }

    public void StartGame()
    {
        GameManager.instance.TryStartGame();
    }

    #region InGame UserInterface
    //Updates The Score Text from Game Manger Score Value
    public void UpdateScoreText()
    {
        if(GameManager.instance != null)
            Txt_Score.text = GameManager.instance.TotalScore.ToString();
    }

    //Activates or Deactivates the Main Menu
    public void SetGameUIActive(bool bActive = true)
    {
        PanelInGame.gameObject.SetActive(bActive);
    }
    #endregion

    #region PostGame UserInterface

    //Activates or Deactivates the Post Game
    public void SetPostGameUIActive(bool bActive = true)
    {
        PanelPostGame.gameObject.SetActive(bActive);
    }

    //Sets Result to the Post Game UI
    public void InitlizePostGameResult(string newMessage, int newTotalScore , int newTotalFruits)
    {
        //Message
        Txt_PostMessage.text = newMessage;

        //Total Score
        Txt_TotalScore.text = newTotalScore.ToString();

        //Total Fruits
        Txt_TotalFruits.text = newTotalFruits.ToString();
    }

    //Tries to Restart the Game
    public void RetryGame()
    {
        if (GameManager.instance != null)
            GameManager.instance.TryStartGame();
    }
    #endregion

    #region Menu UserInterface
    //Activates or Deactivates the Main Menu
    public void SetMenuActive(bool bActive = true)
    {
        PanelMainMenu.gameObject.SetActive(bActive);
    }


    #region Setting UI
    //Toggles Between Setting Open and Close State
    public void ToggleSetting()
    {
        //Open/Close Setting
        PanelSettings.gameObject.SetActive(!PanelSettings.gameObject.activeSelf);

        UpdateSettingUI();

    }

    //Called by SFX Slider Volume When a new Value is Set
    public void OnNewSoundVolume()
    {
        if (Slider_SFX != null &&
            GameManager.instance != null)
        {
            GameManager.instance.VolSfx = Slider_SFX.value;
            GameManager.instance.SaveSetting();

            if (Utility.UtilInstance != null)
                Utility.UtilInstance.UpdateSFXVolume();
        }

    }

    //Called by Music Slider Volume When a new Value is Set
    public void OnNewMusicVolume()
    {
        if (Slider_Music != null &&
            GameManager.instance != null)
        {
            GameManager.instance.VolBgm = Slider_Music.value;
            GameManager.instance.SaveSetting();

            //Update Music Player
            Utility.UtilInstance.MusicPlayer.volume = Slider_Music.value;
        }
    }

    //Updates Volume Sliders Value for Music and Sound Effects in Setting
    private void UpdateSettingUI()
    {
        if(GameManager.instance != null)
        {
            if (Slider_SFX != null)
                Slider_SFX.value = GameManager.instance.VolSfx;

            if (Slider_Music != null)
                Slider_Music.value = GameManager.instance.VolBgm;
        }
    }
    #endregion

    //Exits Application
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion 

}
