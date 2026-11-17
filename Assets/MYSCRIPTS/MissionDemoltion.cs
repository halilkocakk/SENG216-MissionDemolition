using UnityEngine;

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Level Settings")]
    public Vector3 castlePos = new Vector3(10, -9.5f, 0);
    public GameObject[] castles;

    [Header("Level Information")]
    public int level;
    public int levelMax;
    public GameObject castle;

    void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;

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

        Goal.goalMet = false;

        // Create the new castle
        castle = Instantiate(castles[level]);
        castle.transform.position = castlePos;

        Debug.Log("Level started: " + (level + 1));

        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void Update()
    {
        // Check whether the goal has been reached
        if (Goal.goalMet)
        {
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            // Consume the hit even if a transition is already scheduled.
            Goal.goalMet = false;

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
        }

        StartLevel();
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }
}