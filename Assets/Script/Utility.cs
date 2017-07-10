using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {
    // Use this for initialization


    [Header("Music")]
    /* Sound Clip for Main Menu Background Music */
    public AudioClip BGM_Menu;

    /* Sound Clip for Game Background Music */
    public AudioClip BGM_Game;

    /* Audio Source Reference */
    public AudioSource MusicPlayer;

    /* Static Global Reference to Instance of Utility Class */
    public static Utility UtilInstance;

    void Awake()
    {
        //Implementing Singleton
        if (UtilInstance == null)
            UtilInstance = this;
        else if (UtilInstance != null)
            Destroy(UtilInstance);
    }

    void Start () {
        MusicPlayer = GetComponent<AudioSource>();
        MusicPlayer.volume = GameManager.instance.VolBgm;
        MusicPlayer.loop = true;
        PlayMusic(BGM_Menu);
        UpdateSFXVolume();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (GameManager.instance.GameState == GameManager.EGameState.Menu)
        {
            Camera.main.transform.Rotate(0, 5 * Time.deltaTime, 0, Space.World);
            Camera.main.transform.position = new Vector3(0, 40, 0);
        }


            
	}

    //Plays the Given sound clip
    public void PlayMusic(AudioClip SoundClip)
    {
        if (MusicPlayer != null &&
            MusicPlayer.clip != SoundClip)
        {
            MusicPlayer.clip = SoundClip;
            MusicPlayer.volume = GameManager.instance.VolBgm;
            MusicPlayer.Play();
        }
            
    }

    //Updates Sound Effect Volume
    public void UpdateSFXVolume()
    {
        AudioSource SFXPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        if (SFXPlayer)
            SFXPlayer.volume = GameManager.instance.VolSfx / 2;

    }

}
