using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private GameManager gameManager;
    private bool addedScore;
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !addedScore)
        {
            gameManager.UpdateScore(1);
        }

        addedScore = true;
    }
}
