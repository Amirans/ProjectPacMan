using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


    private int TotalScore = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Adds Score to the Game Total Score
    public void AddScore(int ScoreValue)
    {
        TotalScore += ScoreValue;
        Debug.Log(ScoreValue + " Score Added, Totaling in: " + TotalScore);
    }
}
