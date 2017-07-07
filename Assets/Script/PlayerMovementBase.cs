﻿using UnityEngine;
using System.Collections;

public class PlayerMovementBase : MonoBehaviour {


    #region Movement

    [Header("Movement Setting")]
    /* Movement Speed Of Player */
    public float moveSpeed = 10;

    /* Player Rigid Body */
    private Rigidbody Controller;

    [Header("Camera Setting")]
    /* Distance of the Camera to the Player */
    public float CameraDistance = 10;

    /* Camera Lag Value */
    public float CameraLag = 2;

    #endregion


    // Use this for initialization
    void Start () {
        Controller = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {


        #region PlayerMovement
        Vector3 NewMovement = new Vector3();

        //Get Axis of Horizontal and Vertical Input
        NewMovement.x = Input.GetAxis("Horizontal");
        NewMovement.z = Input.GetAxis("Vertical");

        //Move the Player
        Controller.MovePosition(transform.position + NewMovement * moveSpeed * Time.deltaTime);
        #endregion


        #region CameraMovement
        //Camera Movement with Lag

        //Calculate the New Camera Position, Respecting the Distance and centering the Player Z Axis
        Vector3 NewCameraPos = new Vector3(transform.position.x, transform.position.y + CameraDistance, transform.position.z - 5.0f);

        //Slerp the Camera Vector From Current Position to the New Camera Position
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, NewCameraPos, Time.deltaTime * CameraLag);
        #endregion
    }
}
