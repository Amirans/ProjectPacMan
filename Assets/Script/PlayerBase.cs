using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {


    #region Properties

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
	void FixedUpdate () {

        #region PlayerMovement
        Vector3 NewMovement = new Vector3();

        //Get Axis of Horizontal and Vertical Input
        NewMovement.x = Input.GetAxis("Horizontal");
        NewMovement.z = Input.GetAxis("Vertical");

        //Check if New Movement is not Zero Vector
        if(NewMovement != Vector3.zero)
        {
            //Set Player Direction forward
            transform.forward = Vector3.Normalize(NewMovement);

            //Move the Player
            Controller.MovePosition(transform.position + NewMovement * moveSpeed * Time.deltaTime);
        }
       
        #endregion


        #region CameraMovement
        //Camera Movement with Lag

        //Calculate the New Camera Position, Respecting the Distance and centering the Player Z Axis
        Vector3 NewCameraPos = new Vector3(transform.position.x, transform.position.y + CameraDistance, transform.position.z - 5.0f);

        //Slerp the Camera Vector From Current Position to the New Camera Position
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, NewCameraPos, Time.deltaTime * CameraLag);
        #endregion
    }

    void OnTriggerEnter(Collider other)
    {
        //if(other.tag == "Pickable" &&
            //other.GetComponent<PickableBase>())
        //{
            //((PickableBase)other.gameObject.GetComponent<PickableBase>()).Pickup(this.gameObject);
        //}else
        if(other.CompareTag("Fruit"))
        {
            //Fruit Collected
            if(GameManager.instance != null)
                GameManager.instance.OnFruitCollected();

            //Destroy Fruit
            Destroy(other.gameObject);
        }
            
    }
}
