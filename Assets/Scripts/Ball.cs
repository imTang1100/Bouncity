using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class Ball: MonoBehaviour
{
    [SerializeField] private GameplaySettings settings;

    private new Rigidbody rigidbody;

    private new SphereCollider collider;

    private AudioSource audioSrc;

    [SerializeField] private AudioClip[] collisions;

    [SerializeField] private AudioClip shotHit;

    public event Action<Facade> OnFacadeCollision;

    private Facade ground;

    private Vector3? oldGndPos;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
        audioSrc = GetComponent<AudioSource>();
        settings.OnUpdated += ApplySettings;
        ApplySettings();
    }

    public Hole Hole
    {
        set => ground = value.Ground;
    }

    private void ApplySettings()
    {
        transform.localScale = settings.ballDiameter * Vector3.one;
        rigidbody.drag = settings.ballDrag;
        collider.sharedMaterial.bounciness = settings.defaultBounciness;
    }

    private void OnDestroy()
    {
        settings.OnUpdated -= ApplySettings;
    }

    private void OnCollisionEnter(Collision o)
    {
        if(audioSrc != null && collisions != null && collisions.Length > 0)
        {
            audioSrc.PlayOneShot(collisions[Random.Range(0, collisions.Length)]);
        }

        Facade facade = o.gameObject.GetComponent<Facade>();

        if(facade != null)
        {
            OnFacadeCollision?.Invoke(facade);
        }
    }

    private void FixedUpdate()
    {
        if (ground != null)
        {
            if (oldGndPos.HasValue)
            {
                rigidbody.position += ground.transform.position - oldGndPos.Value;
            }

            oldGndPos = ground.transform.position;
        }
        else
        {
            oldGndPos = null;
        }
    }

    public float Radius => collider.radius * transform.lossyScale.x;

    public void OnShotHit()
    {
        if(audioSrc != null && shotHit != null)
        {
            audioSrc.PlayOneShot(shotHit);
        }
    }
}
