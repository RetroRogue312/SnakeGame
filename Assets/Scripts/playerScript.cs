using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    public float speed;
    
    private int direction;
    
    public int score;
    public int bestScore;
    
    public GameObject[] fruits;
    public GameObject currentFruit;
    public int fruitIndex;
    private int countBeforeSpeedUp;
    private int countBeforeEnemy;
    private int countBeforeOrange;

    public GameObject infoPanel;

    public GameObject turnPointPrefab;
    public List<GameObject> turnPoints;
    public bool gameStarted;
    public GameObject tail;

    public GameObject bodyPrefab;
    private GameObject currentSegment;
    private List<GameObject> segments;
    
    private float growthBuffer = 0.0f;
    private float growthPerFruit = 1.0f;
    
    public AudioSource[] KeySounds = new AudioSource[4];
    public AudioSource deathSound;
    
    //1 = up
    //2 = down
    //3 = left
    //4 = right
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = 0;
        speed = 2f;
        score = 0;
        gameStarted = false;
        turnPoints = new List<GameObject>();
        segments = new List<GameObject>();
        countBeforeSpeedUp = 5;
        countBeforeEnemy = 10;
        countBeforeOrange = 5;

        fruitIndex = Random.Range(0, fruits.Length);
        currentFruit = fruits[fruitIndex];
        currentFruit.SetActive(true);
        

        CreateSegment();
        stretchCurrentSegment();
        shrinkOldestSegment();
    }

    // Update is called once per frame
    void Update()
    {
        moveHead();
        moveTail();
        if (gameStarted)
        {
            infoPanel.SetActive(false);
        }
        
        stretchCurrentSegment();
        shrinkOldestSegment();

        if (countBeforeSpeedUp == 0)
        {
            speed += 2f;
            countBeforeSpeedUp = 5;
        }

        if (countBeforeOrange == 0)
        {
            
        }
    }

    void moveHead()
    {
        Vector2 newPos = transform.position;
        if ((Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && direction != 2 && direction != 1)
        {
            direction = 1;
            gameStarted = true;
            transform.eulerAngles = new Vector3(0,0,-90);
            KeySounds[0].Play();
            addPoint();
            
        }

        if ((Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) && direction != 1 && direction != 2)
        {
            direction = 2;
            gameStarted = true;
            transform.eulerAngles = new Vector3(0,0,90);
            KeySounds[1].Play();
            addPoint();
           
            
        }

        if ((Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) && direction != 4 && direction != 3)
        {
            direction = 3;
            gameStarted = true;
            transform.eulerAngles = new Vector3(0,0,0);
            KeySounds[2].Play();
            addPoint();
        }
        
        if ((Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) && direction != 3 && direction != 4)
        {
            direction = 4;
            gameStarted = true;
            transform.eulerAngles = new Vector3(0,0,180);
            KeySounds[3].Play();
            addPoint();
        }
        
        if (direction == 1)
        {
            newPos.y = newPos.y + speed * Time.deltaTime;
        }
        
        if (direction == 2)
        {
            newPos.y = newPos.y - speed * Time.deltaTime;
        }

        if (direction == 3)
        {
            newPos.x = newPos.x - speed * Time.deltaTime;
        }
        
        if (direction == 4)
        {
            newPos.x = newPos.x + speed * Time.deltaTime;
        }
        transform.position = newPos;
    }
    
    void moveTail()
    {


        if (!gameStarted)
            return;
        
        GameObject target;
        if (turnPoints.Count > 0)
        {
            target = turnPoints[turnPoints.Count - 1];
        }
        else
        {
            target = gameObject;
        }
           
        float step = speed * Time.deltaTime;

        if (growthBuffer > 0)
        {
            growthBuffer -= step;
            if (growthBuffer < 0)
                growthBuffer = 0;
        }
        else
        {
            tail.transform.position = Vector2.MoveTowards(tail.transform.position, target.transform.position, speed * Time.deltaTime);
            if (target != gameObject && Vector2.Distance(tail.transform.position, target.transform.position) < 0.001f)
            {
                tail.transform.position = target.transform.position;
                turnPoints.Remove(target);
                Destroy(target);

                if (segments.Count > 1)
                {
                    GameObject oldSegment = segments[segments.Count - 1];
                    segments.Remove(oldSegment);
                    Destroy(oldSegment);
                }
            }
        }
    }

    void CreateSegment()
    {
        currentSegment = Instantiate(bodyPrefab);
        
        if (currentSegment.TryGetComponent<BoxCollider2D>(out BoxCollider2D collider))
        {
            collider.enabled = false; 
        }
        segments.Insert(0, currentSegment);
    }

    void stretchCurrentSegment()
    {
        if (!gameStarted || currentSegment == null)
            return;
        Vector2 start = transform.position;
        Vector2 end;
        if (turnPoints.Count > 0)
        {
            end = turnPoints[0].transform.position;
        }
        else
        {
            end = tail.transform.position;
        }
        AlignSegment(currentSegment, start, end);
    }

    void shrinkOldestSegment()
    {
        if (segments.Count <= 1)
            return;

        GameObject oldest = segments[segments.Count - 1];
        Vector2 start = turnPoints[turnPoints.Count - 1].transform.position;
        Vector2 end = tail.transform.position;
        
        AlignSegment(oldest, start, end);
    }

    void AlignSegment(GameObject segment, Vector2 start, Vector2 end)
    {
        segment.transform.position = (start + end) / 2f;
        Vector2 direction = end - start;
        segment.transform.right = direction;
        float distance = Vector2.Distance(start, end);
        
        Vector3 scale = segment.transform.localScale;
        scale.x = distance;
        segment.transform.localScale = scale;
    }

    void addPoint()
    {
        GameObject point = Instantiate(turnPointPrefab,transform.position,Quaternion.identity);
        turnPoints.Insert(0, point);
        if (segments.Count > 4)
        {
            if (segments[4].TryGetComponent<BoxCollider2D>(out BoxCollider2D collider))
            {
                collider.enabled = true;
                print("Collider enabled on segment 4!");
            }
        }
        CreateSegment();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("fruit"))
        {
            score += 1;
            
            
            if (collision.transform.parent != null && collision.transform.parent.CompareTag("trap"))
            {
                collision.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
            
            int newIndex = Random.Range(0, fruits.Length);
            
            while (newIndex == fruitIndex)
                newIndex = Random.Range(0, fruits.Length);
            
            fruitIndex = newIndex;
            currentFruit = fruits[fruitIndex];
            currentFruit.SetActive(true);
            
            
            print("fruit count: " + score);
            growthBuffer += growthPerFruit;
            countBeforeEnemy--;
            countBeforeOrange--;
            countBeforeSpeedUp--;

        }

        if (collision.gameObject.CompareTag("obstacle"))
        {
            deathSound.Play();
            speed = 0.0f;
            print("Game over");
            gameStarted = false;
        }
    }
}
