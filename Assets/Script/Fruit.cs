﻿using UnityEngine;
using System.Collections;

public class Fruit : PickableBase
{


    [Header("Setting")]
    [SerializeField]
    //The amount of Score Added by This Fruit
    private int Score = 0;

    public GameManager Manager;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool Pickup(GameObject Instigator)
    {
        if (Instigator != null &&
            Instigator.tag == "Player" &&
            Manager != null)
        {
            Manager.AddScore(Score);
            //TODO: Call Total Fruit/Objective Check
            Destroy(this.gameObject);
            return true;
        }

        return false;
    }
}