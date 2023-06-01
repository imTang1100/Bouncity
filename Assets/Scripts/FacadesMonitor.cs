using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.XR.ARCoreExtensions;

public class FacadesMonitor:MonoBehaviour
{
    [SerializeField] private Facade buildingPrefab;
    [SerializeField] private Facade terrainPrefab;
    [SerializeField] private GameplaySettings settings;

    private readonly Dictionary<ARStreetscapeGeometry, Facade> facades = new();

    public event Action<Facade> OnFacadeAdded;

    private ARStreetscapeGeometryManager geometryManager;

    private readonly List<Color> colorsLeft = new();

    private readonly Color[] colors =
    {
        new(66f / 255f, 133f / 255f, 234f / 255f), // google blue 
        new(219f / 255f, 68f / 255f, 55f / 255f), // google red
        new(244f / 255f, 160f / 255f, 0f / 255f), // google yellow
        new(15f / 255f, 157f / 255f, 88f / 255f) // google green
    };

    private void Awake()
    {
        geometryManager = GetComponent<ARStreetscapeGeometryManager>();
    }

    private Color NextRandomColor()
    {
        if (colorsLeft.Count <= 0)
        {
            var rnd = new System.Random();
            var colorsShuffled = colors.OrderBy(color => rnd.Next());
            colorsLeft.AddRange(colorsShuffled);
        }

        Color toReturn = colorsLeft[0];
        colorsLeft.RemoveAt(0);
        return toReturn;
    }


    private void OnFacadesChanged(ARStreetscapeGeometriesChangedEventArgs args)
    {
        if (args.Removed != null)
        {
            foreach (var removedFacade in args.Removed)
            {
                if (facades.TryGetValue(removedFacade, out var facade))
                {
                    Destroy(facade.gameObject);
                    facades.Remove(removedFacade);
                }
            }
        }

        if (args.Added != null)
        {
            foreach (var addedFacade in args.Added)
            { 
                Facade prefab;
                switch (addedFacade.streetscapeGeometryType)
                {
                    case StreetscapeGeometryType.Terrain:
                        prefab = terrainPrefab;
                        break;
                    case StreetscapeGeometryType.Building:
                        prefab = buildingPrefab;
                        break;
                    default:
                        throw new ArgumentException();
                }
                var facade = Instantiate(prefab);
                var mesh = addedFacade.mesh;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                facade.Mesh = mesh;
                facade.GeoType = addedFacade.streetscapeGeometryType;
                facade.Color = addedFacade.streetscapeGeometryType == StreetscapeGeometryType.Building
                    ? NextRandomColor()
                    : Color.white;
                facade.trackID = addedFacade.trackableId;
                facade.gameObject.name = addedFacade.trackableId.ToString();
                facade.transform.position = addedFacade.pose.position +
                                            (addedFacade.streetscapeGeometryType == StreetscapeGeometryType.Terrain
                                                ? Vector3.down * settings.floorOffset
                                                : Vector3.zero);
                facade.transform.rotation = addedFacade.pose.rotation;

                facades[addedFacade] = facade;

                OnFacadeAdded?.Invoke(facade);
            }
        }

        if (args.Updated != null)
        {
            foreach (var updatedFacade in args.Updated)
            {
                if (facades.TryGetValue(updatedFacade, out var facade))
                {
                    facade.transform.position = updatedFacade.pose.position +
                                                (updatedFacade.streetscapeGeometryType == StreetscapeGeometryType.Terrain
                                                    ? Vector3.down * settings.floorOffset
                                                    : Vector3.zero);
                    facade.transform.rotation = updatedFacade.pose.rotation;
                }
            }
        }
    }

    public IEnumerable<Facade> Facades => facades.Values;

    private void Start()
    {
        geometryManager.StreetscapeGeometriesChanged += OnFacadesChanged;
        
    }

    private void OnDestroy()
    {
        geometryManager.StreetscapeGeometriesChanged -= OnFacadesChanged;
    }
}
