using UnityEngine;
using System.Collections;

public class PickableBase : MonoBehaviour {


    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Called to Pickup this Object
    /// </summary>
    /// <param name="Instigator">The Object Instigating/Interacting with this pickable</param>
    public virtual bool Pickup(GameObject Instigator)
    {
        return false;
    }




}
