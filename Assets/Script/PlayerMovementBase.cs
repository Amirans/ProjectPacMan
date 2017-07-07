using UnityEngine;
using System.Collections;

public class PlayerMovementBase : MonoBehaviour {


    #region Movement

    [Header("Movement Setting")]
    /* Movement Speed Of Player */
    public float moveSpeed = 10;

    /* Player Rigid Body */
    private Rigidbody Controller;

    #endregion
    
    
    // Use this for initialization
    void Start () {
        Controller = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {


        #region Movement
        Vector3 NewMovement = new Vector3();

        //Get Axis of Horizontal and Vertical Input
        NewMovement.x = Input.GetAxis("Horizontal");
        NewMovement.z = Input.GetAxis("Vertical");

        //Move the Player
        Controller.MovePosition(transform.position + NewMovement * moveSpeed * Time.deltaTime);
        #endregion
    }
}
