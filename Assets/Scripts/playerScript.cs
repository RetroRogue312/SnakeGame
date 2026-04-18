using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    private double length;
    public float speed;
    
    private int direction;
    
    public int fruitCount;
    public GameObject[] apples;
    public GameObject[] oranges;
    
    
    private int countBeforeSpeedUp;
    private int countBeforeEnemy;
    private int countBeforeOrange;

    public GameObject turnPointPrefab;
    public List<GameObject> turnPoints;
    private bool gameStarted;
    public GameObject tail;

    public GameObject bodyPrefab;
    private GameObject currentSegment;
    private List<GameObject> segments;
    
    
    //1 = up
    //2 = down
    //3 = left
    //4 = right
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = 0;
        speed = 1.5f;
        fruitCount = 0;
        gameStarted = false;
        turnPoints = new List<GameObject>();
        segments = new List<GameObject>();
        countBeforeSpeedUp = 5;
        countBeforeEnemy = 10;
        countBeforeOrange = 5;
        
        

        CreateSegment();
        stretchCurrentSegment();
        shrinkOldestSegment();
    }

    // Update is called once per frame
    void Update()
    {
        moveHead();
        moveTail();
        
        
        stretchCurrentSegment();
        shrinkOldestSegment();

        if (countBeforeSpeedUp == 0)
        {
            speed += 0.5f;
            countBeforeSpeedUp = 5;
        }

        if (countBeforeOrange == 0)
        {
            
        }
    }

    void moveHead()
    {
        Vector2 newPos = transform.position;
        if ((Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && direction != 2)
        {
            direction = 1;
            gameStarted = true;
            addPoint();
            
        }

        if ((Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) && direction != 1)
        {
            direction = 2;
            gameStarted = true;
            addPoint();
           
            
        }

        if ((Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) && direction != 4)
        {
            direction = 3;
            gameStarted = true;
            addPoint();
        }
        
        if ((Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) && direction != 3)
        {
            direction = 4;
            gameStarted = true;
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
        GameObject target;
        if (turnPoints.Count > 0)
        {
            target = turnPoints[turnPoints.Count - 1];
        }
        else
        {
            target = gameObject;
        }
        if (gameStarted)
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

    void CreateSegment()
    {
        currentSegment = Instantiate(bodyPrefab);
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
        
        CreateSegment();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("fruit"))
        {
            fruitCount += 1;
            print("fruit count: " + fruitCount);
            collision.gameObject.SetActive(false);
            countBeforeEnemy--;
            countBeforeOrange--;
            countBeforeSpeedUp--;

        }

        if (collision.gameObject.CompareTag("obstacle"))
        {
            speed = 0.0f;
            print("Game over");
        }
        if (collision.gameObject.CompareTag("citric"))
        {
            //decrease snake's size.
        }

    }
}
