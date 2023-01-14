using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public float powerFactor = 100f;
    public int TrajectoryPosCount = 50;

    private Vector2 defaultBallPosition;
    private Vector2 startPos, endPos;
    private Rigidbody2D rb2;

    private float ballScorePos;

    private Scene mainScene;
    private Scene predictionScene;

    private PhysicsScene2D mainScenePhysics;
    private PhysicsScene2D predictionScenePhysics;

    public UnityEvent scoreEvent;

    public GameObject predictionBall;

    private void Awake()
    {
        //Debug.Log("awaken");
        rb2 = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        //Debug.Log("enabled");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("started");
        Physics2D.simulationMode = SimulationMode2D.Script;

        rb2.isKinematic = true;
        defaultBallPosition = transform.position;

        CreateMainScene();
        CreatePredictionScene();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos = GetMousePos();
            //Debug.Log(startPos);
        }

        if(Input.GetMouseButton(0))
        {
            GameObject newPredictionBall = SpawnPredictionBall();

            ThrowBall(newPredictionBall.GetComponent<Rigidbody2D>());
            CreateTrajectory(newPredictionBall);

            Destroy(newPredictionBall);
        }

        if(Input.GetMouseButtonUp(0))
        {
            GetComponent<LineRenderer>().positionCount = 0;
            
            rb2.isKinematic = false;
            ThrowBall(rb2);
        }
    }

    void FixedUpdate()
    {
        if (!mainScenePhysics.IsValid()) return;
        //if (!predictionScenePhysics.IsValid()) return;

        mainScenePhysics.Simulate(Time.fixedDeltaTime);
        //predictionScenePhysics.Simulate(Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        ballScorePos = transform.position.y;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(transform.position.y < ballScorePos)
        {
            scoreEvent.Invoke();
            //Debug.Log("Score");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.tag.Equals("ground")) return;

        rb2.isKinematic = true;
        transform.position = defaultBallPosition;
        rb2.velocity = Vector2.zero;
        rb2.angularVelocity = Vector2.zero.x;
    }

    private void CreateTrajectory(GameObject newPredictionBall)
    {
        LineRenderer ballLine = GetComponent<LineRenderer>();
        ballLine.positionCount = TrajectoryPosCount;

        for (int i = 0; i < ballLine.positionCount; i++)
        {
            predictionScenePhysics.Simulate(Time.fixedDeltaTime);
            ballLine.SetPosition(i, new Vector3(newPredictionBall.transform.position.x, newPredictionBall.transform.position.y, 0));
        }
    }

    private void ThrowBall(Rigidbody2D rb2)
    {
        rb2.AddForce(GetPowerOfThrow(startPos, GetMousePos()), ForceMode2D.Force);
    }

    private GameObject SpawnPredictionBall()
    {
        GameObject newPredictionBall = GameObject.Instantiate(predictionBall);
        SceneManager.MoveGameObjectToScene(newPredictionBall, predictionScene);
        newPredictionBall.transform.position = transform.position;
        return newPredictionBall;
    }

    private Vector2 GetPowerOfThrow(Vector2 startPos, Vector2 endPos)
    {
        return (startPos - endPos) * powerFactor;
    }

    private Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void CreateMainScene()
    {
        mainScene = SceneManager.CreateScene("MainScene");
        mainScenePhysics = mainScene.GetPhysicsScene2D();
    }

    private void CreatePredictionScene()
    {
        CreateSceneParameters sceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        predictionScene = SceneManager.CreateScene("PredictionScene", sceneParameters);
        predictionScenePhysics = predictionScene.GetPhysicsScene2D();
    }
}
