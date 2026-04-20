using UnityEngine;

public class ScoreHolder : MonoBehaviour
{
    public static ScoreHolder instance;

    public int score;
    public int best;
    
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        best = 0;
    }

    int getBest()
    {
        return score;
    }

    int Score()
    {
        return score;
    }

    void updateScore(int s)
    {
        score = s;
    }

    void updateBest(int s)
    {
        best = s;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
