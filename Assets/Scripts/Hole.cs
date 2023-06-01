using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;


public class Hole: MonoBehaviour
{
    [SerializeField] private TriggerPublish ballInTrig;

    [SerializeField] private TriggerPublish ballOnTopTrig;

    [SerializeField] private GameplaySettings settings;

    [SerializeField] private float ballDestoryDelay;

    private Vector3 sphereColliderCtr;

    private bool ballInside;

    private Facade ground;

    private SphereCollider ballOnTopCollider;

    private event Action<Ball> onBallIn;

    private readonly HashSet<Ball> ballsOnTop = new();

    public Facade Ground
    {
        get => ground;

        set
        {
            ground = value;
        }
    }

    private void RefreshSize()
    {
        float localScale = 2f * settings.holeRadius / ballOnTopCollider.transform.localScale.x;
        transform.localScale = localScale * Vector3.one;
    }

    private void Awake()
    {
        ballOnTopCollider = ballOnTopTrig.GetComponent<SphereCollider>();
        settings.OnUpdated += RefreshSize;
        ballInTrig.OnStateChanged += OnBallInChanged;
        ballOnTopTrig.OnStateChanged += OnBallOnTop;
    }

    private void Start()
    {
        RefreshSize();
    }

    private void OnBallOnTop(bool isBallOnTop, Collider ballCollider)
    {
        int newLayer = LayerMask.NameToLayer(isBallOnTop ? "Fall" : "Ball");
        Transform ballRoot = ballCollider.transform.root;
        ballRoot.gameObject.SetLayerRecursively(newLayer);
        Ball ball = ballRoot.GetComponentInChildren<Ball>();
        if (isBallOnTop)
        {
            ballsOnTop.Add(ball);
        } else
        {
            ballsOnTop.Remove(ball);
        }
    }

    public void AddBallInListener(Action<Ball> onBallIn)
    {
        this.onBallIn += onBallIn;
    }

    public void RemoveBallInListener(Action<Ball> onBallIn)
    {
        this.onBallIn -= onBallIn;
    }

    private IEnumerator DestroyBall(Ball ball, float delay)
    {
        //Delete it with delay time t
        yield return new WaitForSeconds(delay);
        if (ball != null)
        {
            Destroy(ball.gameObject);
        }
    }

    private void OnBallInChanged(bool isBallIn, Collider ballCollider)
    {
        if(isBallIn)
        {
            Ball ball = ballCollider.transform.root.GetComponent<Ball>();
            onBallIn?.Invoke(ball);
            StartCoroutine(DestroyBall(ball, ballDestoryDelay));
        }
    }

    public bool IsOnTop(Ball ball)
    {
        return ballsOnTop.Contains(ball);
    }

    public Vector3 Origin => ballOnTopCollider.transform.position;

    public float Radius => ballOnTopCollider.radius * ballOnTopCollider.transform.lossyScale.x;

    private void OnDestroy()
    {
        settings.OnUpdated -= RefreshSize;
    }
}
