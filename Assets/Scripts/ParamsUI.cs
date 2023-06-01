using System;
using TMPro;
using UnityEngine;

public class ParamsUI : MonoBehaviour
{
    [SerializeField]
    private GameplaySettings settings;

    [SerializeField]
    private Transform paramsContent;

    [SerializeField]
    private TMP_InputField forcePerSecond;

    [SerializeField]
    private TMP_InputField ballDiameter;

    [SerializeField]
    private TMP_InputField defaultBounciness;

    [SerializeField]
    private TMP_InputField ballSpawnDistance;

    [SerializeField]
    private TMP_InputField ballSpawnHeight;

    [SerializeField]
    private TMP_InputField holeAngle;

    [SerializeField]
    private TMP_InputField holeSize;

    [SerializeField]
    private TMP_InputField buildingOpacityOnHit;

    [SerializeField]
    private TMP_InputField buildingOpacityDefault;

    [SerializeField]
    private TMP_InputField floorOffset;
    

    private void Awake()
    {
       paramsContent.gameObject.SetActive(false);
       
    }

    private void ApplyParams()
    {
        try
        {
            settings.forcePerSecond = float.Parse(forcePerSecond.text);
            settings.ballDiameter = float.Parse(ballDiameter.text);
            settings.defaultBounciness = float.Parse(defaultBounciness.text);
            settings.ballSpawnDistance = float.Parse(ballSpawnDistance.text);
            settings.ballSpawnHeight = float.Parse(ballSpawnHeight.text);
            settings.holeAngle = float.Parse(holeAngle.text);
            settings.holeRadius = float.Parse(holeSize.text);
            settings.buildingOpacityOnHit = float.Parse(buildingOpacityOnHit.text);
            settings.buildingOpacityDefault = float.Parse(buildingOpacityDefault.text);
            settings.floorOffset = float.Parse(floorOffset.text);

            settings.PublishUpdates();
        }
        catch (FormatException)
        {

        }
    }

    private void LoadParams()
    {
        forcePerSecond.text = settings.forcePerSecond.ToString();
        ballDiameter.text = settings.ballDiameter.ToString();
        defaultBounciness.text = settings.defaultBounciness.ToString();
        ballSpawnDistance.text = settings.ballSpawnDistance.ToString();
        ballSpawnHeight.text = settings.ballSpawnHeight.ToString();
        holeAngle.text = settings.holeAngle.ToString();
        holeSize.text = settings.holeRadius.ToString();
        buildingOpacityOnHit.text = settings.buildingOpacityOnHit.ToString();
        buildingOpacityDefault.text = settings.buildingOpacityDefault.ToString();
        floorOffset.text = settings.floorOffset.ToString();
    }


    void Start()
    {
        forcePerSecond.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        ballDiameter.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        defaultBounciness.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        ballSpawnDistance.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        ballSpawnHeight.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        holeAngle.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        holeSize.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        buildingOpacityOnHit.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        buildingOpacityDefault.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        floorOffset.characterValidation = TMP_InputField.CharacterValidation.Decimal;

        LoadParams();
    }

    private void OnEnable()
    {
        Input.location.Start();
    }

    private void OnDisable()
    {
        Input.location.Stop();
    }


    public void OnOpenClicked()
    {
        paramsContent.gameObject.SetActive(true);
    }

    public void OnCloseClicked()
    {
        paramsContent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!paramsContent.gameObject.activeInHierarchy) return;

        ApplyParams();
    }
}
