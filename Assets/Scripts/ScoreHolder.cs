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
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    


    // Update is called once per frame
    void Update()
    {
        
    }
}
