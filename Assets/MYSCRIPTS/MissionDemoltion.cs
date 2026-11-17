using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;




public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; //Singelton

     [Header("Inscribed")]
    public Text uitLevel;              // UIText_Level Text referansı
    public Text uitShots;  
    public Vector3 castlePos = new Vector3(50, -9.5f, 0);
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public GameObject castle;
    public int shotsTaken;   
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; 

    void Start()
    {
        S = this;//Singelton

        level = 0;
        levelMax = castles.Length;
        shotsTaken = 0;

        if (levelMax == 0)
        {
            Debug.LogError("No castle prefabs assigned!");
            return;
        }

        StartLevel();
    }

    void StartLevel()
    {
        // Remove the previous castle
        if (castle != null)
        {
            castle.SetActive(false);
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        // Create the new castle
        if (castles != null && castles.Length > 0) {
            castle = Instantiate<GameObject>(castles[level], castlePos, Quaternion.identity);
        }

        Goal.goalMet = false;

        
        UpdateGUI();
        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);

       
    }
     void UpdateGUI() {
        
        if (uitLevel != null) {
            uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        }
        if (uitShots != null) {
            uitShots.text = "Shots Taken: " + shotsTaken;
        }
    }

    void Update()
    {
        UpdateGUI();
        // Check whether the goal has been reached
        if ((mode== GameMode.playing) && Goal.goalMet)
        {
            // Consume the hit even if a transition is already scheduled.
            Goal.goalMet = false;
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            // Load the next level after 2 seconds
            if (!IsInvoking(nameof(NextLevel)))
                Invoke(nameof(NextLevel), 2f);
            
        }
    }

    void NextLevel()
    {
        Projectile.DESTROY_PROJECTILES();
        level++;

        // Restart from the first level after the last one
        if (level >= levelMax)
        {
            level = 0;
            shotsTaken = 0;
        }

        StartLevel();
    }
    static public void SHOT_FIRED() {
        if (S != null) {
            S.shotsTaken++;
        }
    }
    static public GameObject GET_CASTLE() {
        return (S != null) ? S.castle : null;
    }
}