using System;
using UnityEngine;
using System.Collections;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARSubsystems;

public class Facade : MonoBehaviour
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int HoleCtrColor = Shader.PropertyToID("_HoleCenter");
    private static readonly int Radius = Shader.PropertyToID("_HoleRadius");

    private enum State
    {
        Transparent,
        Highlighted,
        Invisible,
    }

    private new Renderer renderer;

    private MeshFilter meshFilter;

    private new MeshCollider collider;

    [SerializeField] private MeshFilter shadowCatcher;

    [SerializeField] private GameplaySettings settings;

    [SerializeField] private float pulseDuration;

    private State currentState;

    public StreetscapeGeometryType GeoType { get; set;}

    public TrackableId trackID { get; set; }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        collider = GetComponent<MeshCollider>();
        renderer.material = new Material(renderer.sharedMaterial);
    }

    private void SetState(State newState)
    {
        Color color = renderer.material.GetColor(BaseColor);
        if (newState != State.Invisible)
        {
            color.a = newState == State.Highlighted ? settings.buildingOpacityOnHit : settings.buildingOpacityDefault;
            renderer.enabled = true;
        } else
        {
            color.a = 0f;
        }

        renderer.material.SetColor(BaseColor, color);
        currentState = newState;
    }

    public void SetTransparent() => SetState(State.Transparent);
    public void SetHighlighted() => SetState(State.Highlighted);
    public void SetInvisible() => SetState(State.Invisible);

    public Mesh Mesh
    {
        set
        {
            meshFilter.mesh = value;
            shadowCatcher.mesh = value;
            collider.sharedMesh = value;
        }
    }

    public Color Color
    {
        set => renderer.material.SetColor(BaseColor, value);
    }

    private void RefreshAlpha()
    {
        switch (currentState)
        {
            case State.Highlighted:
                SetHighlighted();
                break;
            case State.Transparent:
                SetTransparent();
                break;
            case State.Invisible:
                SetInvisible();
                break;
        }
    }


    private IEnumerator PulseCoroutine()
    {
        float endAlpha;
        switch (currentState)
        {
            case State.Transparent:
                endAlpha = settings.buildingOpacityDefault;
                break;

            case State.Highlighted:
                yield break;

            case State.Invisible:
                endAlpha = 0f;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        State startState = currentState;
        SetState(State.Highlighted);
        Color color = renderer.material.GetColor(BaseColor);
        float startAlpha = color.a;
        float startTime = Time.timeSinceLevelLoad;
        float elapsed;
        while((elapsed = Time.timeSinceLevelLoad - startTime) <= pulseDuration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / pulseDuration);
            renderer.material.SetColor(BaseColor, color);
            yield return null;
        }

        if (startState == State.Invisible)
        {
            SetInvisible();
        } else
        {
            SetState(startState);
        }

    }

    public void Pulse()
    {
        StartCoroutine(PulseCoroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ball")) return;

        if (GeoType == StreetscapeGeometryType.Building)
        {
            Pulse();
        }
    }

    private void Start()
    {
        settings.OnUpdated += RefreshAlpha;
    }

    private void OnDestroy()
    {
        settings.OnUpdated -= RefreshAlpha;
    }

    public Vector3 HolePosition
    {
        set => renderer.material.SetVector(HoleCtrColor, value);
    }

    public float HoleRadius
    {
        set => renderer.material.SetFloat(Radius, value);
    }

}
