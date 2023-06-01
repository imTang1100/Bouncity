using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameplayController : MonoBehaviour
{
    private const int halve = 2;
    [SerializeField] private float groundRayLength;

    [SerializeField] private float messageReadTime;

    [SerializeField] private Ball ballPrefab;

    [SerializeField] private FacadesMonitor facadesMonitor;

    [SerializeField] private Hole holePrefab;

    [SerializeField] private XROrigin sessionOrigin;

    [SerializeField] private GameplaySettings settings;

    [SerializeField] private float holeAngleIncrement;

    [SerializeField] private Transform welcomePopup;

    [SerializeField] private TextMeshProUGUI welcomePopupText;

    [SerializeField] private WelcomePopupAnimation welcomePopupAnimation;

    [SerializeField] private Button welcomePopupButton;

    [SerializeField] private float welcomePopupDelay;

    [SerializeField] private float welcomeTextAnimationDuration;

    [SerializeField] private RectTransform loadingCourseLabel;

    [SerializeField] private RectTransform tapToShootLabel;

    [SerializeField] private RectTransform holeHitLabel;

    [SerializeField] private RectTransform tryAgainLabel;

    [SerializeField] private Button addObstacleButton;

    [SerializeField] private RectTransform cantPlaceHoleLabel;

    [SerializeField] private float obstacleAngleIncrement;

    [SerializeField] private int raycastsPerFrame;

    [SerializeField] private Obstacle[] obstaclePrefabs;

    [SerializeField] private float obstacleRayMaxDistance;

    [SerializeField] private RectTransform cantPlaceObstacleLabel;

    [SerializeField] private RectTransform notBilliardsLabel;

    [SerializeField] private float shotPredictionTime;

    [SerializeField] private float shotMinDuration;

    [SerializeField] private float shotMaxDuration;

    [SerializeField] private RectTransform powerBar;

    [SerializeField] private float powerBarMaxWidth;

    [SerializeField] private float powerBarMaxPower;

    [SerializeField] private float playerActiveLingerTime;

    [SerializeField] private AudioSource music;

    [SerializeField] private AudioSource holeHitSound;

    [Range(0f, 1f)] [SerializeField] private float musicLowVolumePercent;

    [SerializeField] private ParticleSystem confettiParticles;

    [SerializeField] private AudioSource shotChargingAudio;

    private Ball activeBall;
    private Rigidbody activeBallRigidbody;

    private float shootForce;

    private bool shotCharging;
    private bool shotFired;
    private bool freshHole = true;
    private Transform ballPositionTransform;

    private readonly List<Ball> oldBalls = new();

    public Vector3 BallPosition => ballPositionTransform.position;

    public event Action OnNewCourse;

    public event Action OnReset;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        facadesMonitor.OnFacadeAdded += OnFacadeAdded;
    }

    // Update is called once per frame
    void Update()
    {
        if (shotCharging)
        {
            shootForce += Time.deltaTime * settings.forcePerSecond * 100f;
        }
    }

    private void OnDestroy()
    {
        facadesMonitor.OnFacadeAdded -= OnFacadeAdded;
    }

    private void NewGame()
    {
        foreach (Facade facade in facadesMonitor.Facades)
        {
            facade.SetTransparent();
            facade.Pulse();
        }

        SpawnNewBall();
    }


    public void Reset()
    {
        if (activeBall != null)
        {
            Destroy(activeBall.gameObject);
        }

        resetButton.gameObject.SetActive(true);

        OnReset?.Invoke();

        NewGame();
    }

    //1
    public void OnTouchDown()
    {
        if (!shotFired)
        {
            shotCharging = true;
            shootForce = 0f;
        }

    }


    //2
    public void OnTouchUp()
    {
        if (shotCharging)
        {
            Vector3 direction = Vector3.ProjectOnPlane(activeBall.transform.position - Camera.main.transform.position,
                Vector3.up);
            direction.y = 0f;
            activeBallRigidbody.constraints = RigidbodyConstraints.None;
            activeBallRigidbody.AddForce(Vector3.up * shootForce / halve, ForceMode.Impulse);
            activeBallRigidbody.AddForce(direction.normalized * shootForce, ForceMode.Impulse);
            shootForce = 0f;
            
            shotCharging = false;
            freshHole = false;
            shotFired = true;
           
            activeBall.OnShotHit();
        }
    }

    //3
    private void SpawnNewBall()
    {
        if (activeBall != null)
        {
            oldBalls.Add(activeBall);
        }

        if (freshHole)
        {
            Vector3 ballPosition = NewBallPosition(out Transform parent);

            if (ballPositionTransform == null)
            {
                ballPositionTransform = new GameObject("Ball Position").GetComponent<Transform>();
            }

            ballPositionTransform.position = ballPosition;
            ballPositionTransform.parent = parent;
        }

        Vector3 ballSpawnPosition =
            ballPositionTransform.position + Vector3.up * settings.ballSpawnHeight;
        activeBall = Instantiate(ballPrefab, ballSpawnPosition, Quaternion.identity);
        activeBallRigidbody = activeBall.GetComponent<Rigidbody>();

        activeBallRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                                          RigidbodyConstraints.FreezeRotation;

        shotFired = false;

    }

    //4
    private Vector3 NewBallPosition(out Transform parent)
    {
        Vector3 horizontalForward = Camera.main.transform.forward;
        horizontalForward.y = 0f;
        Vector3 position = Camera.main.transform.position + horizontalForward * settings.ballSpawnDistance;
        Vector3 rayStart = position + Vector3.up * groundRayLength;
        Vector3 rayDirection = Vector3.down;
        float rayLength = groundRayLength * 2f + sessionOrigin.CameraYOffset;
        Ray ray = new Ray(rayStart, rayDirection);
        parent = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, rayLength, LayerMask.GetMask("Ground")))
        {
            position = hitInfo.point;
            parent = hitInfo.transform;
        }

        return position;
    }

    private void OnFacadeAdded(Facade newFacade)
    {
        newFacade.SetInvisible();
    }

    public void StartGame()
    {
        startButton.gameObject.SetActive(false);
        NewGame();
    }
}
