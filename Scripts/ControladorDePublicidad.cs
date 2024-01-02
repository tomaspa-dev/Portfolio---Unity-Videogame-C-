using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ControladorDePublicidad : MonoBehaviour
{
    public string gameId_ios;
    public string gameId_android;
    public bool testMode;
    private string gameId; // Set in Start()
    private const string placementId_interstitial = "Omisible";
    private const string placementId_rewarded = "Recompensa";

    // For the purpose of this tutorial
    private int rewardsGiven = 0;
    //public Text rewardDisplayText;

    void Start()
    {
        // Advertisement.Initialize("Recompensa");
        //Advertisement.
    }   
}

