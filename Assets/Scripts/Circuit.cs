﻿using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using SDD.Events;
using Spline;
using Random = UnityEngine.Random;

public class Circuit : MonoBehaviour, IEventHandler
{
    #region Settings & Prefabs

    [field: Header(("LevelDesign"))] 
    [SerializeField] private BezierSpline levelBaseSpline; 
    public BezierSpline LevelBaseSpline => levelBaseSpline;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private Material groundMaterial;

    [Header(("LevelSettings"))] 
    [SerializeField] public int MaxLaps = 1;
    [SerializeField][Range(1,19)] private int numberOfOpponents = 1;

    [Header(("LevelGeneralPrefabs"))]
    [SerializeField] private GameObject[] carPrefabs;
    [SerializeField] private GameObject[] bonusPrefabs;
    
    [Header(("ScenePrefabs"))]
    [SerializeField] private GameObject finishingLinePrefab;
    [SerializeField] private GameObject turnSignPrefab;
    #endregion

    #region Events' subscription
    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }
    #endregion
    
    #region MonoBehaviour lifecycle
    private void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        GenerateCircuit();
    }
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Level generation
    private GameObject levelElements, wallsGO, bonusesGO,
        playersGO, carsGO, groundGO, roadGO, finishingLineGO;

    private void GenerateCircuit()
    {
        //Level static scene elements
        levelElements = new GameObject("Level Scene Elements");
        levelElements.transform.SetParent(transform);
        
        //Moveable elements
        carsGO = new GameObject("Cars");
        carsGO.transform.SetParent(transform);

        //Generate Ground
        groundGO = GenerateElementFromSpline("Ground", 150, groundMaterial);
        groundGO.transform.position = new Vector3(0,-0.1f,0);
        
        //Generate Road
        roadGO = GenerateElementFromSpline("Road", 20, roadMaterial);
        
        //Adding Finishing Line
        finishingLineGO =  Instantiate(finishingLinePrefab, levelElements.transform);
        finishingLineGO.transform.position = new Vector3(0,0.1f,0);
        finishingLineGO.transform.localScale += new Vector3(0,0,2);
        finishingLineGO.transform.Rotate(0,-90,0);
        
        //Generate start positions
        List<Vector3> startPositions = GenerateStartPositions();
        Vector3 playerStartPosition = 
            startPositions[Random.Range(0, startPositions.Count)];
        startPositions.Remove(playerStartPosition);

        //Add player
        GameObject playerGO = Instantiate(carPrefabs[0], playerStartPosition, 
            Quaternion.identity, carsGO.transform);
        playerGO.transform.Rotate(-90,0,0);
        playerGO.AddComponent<Player>();
        playerGO.AddComponent<PlayerController>();
        playerGO.AddComponent<Racer>();
        
        //Add AI opponents
        /*for (int i = 0; i < numberOfOpponents; i++)
        {
            GameObject carGO = Instantiate(
                carPrefabs[Random.Range(0, carPrefabs.Length)], startPositions[i], 
                Quaternion.identity, carsGO.transform);
            carGO.AddComponent<AIController>();
            carGO.AddComponent<Racer>();
        }*/

        //Setting up camera
        Camera camera = FindObjectOfType<Camera>();
        Transform ct = camera.transform;
        ct.SetParent(playerGO.transform);
        ct.localPosition = new Vector3(0,0.1f, 0.04f);
        ct.localRotation = Quaternion.identity;
        ct.Rotate(90,0,0);
        ct.localScale = Vector3.one;
        

        EventManager.Instance.Raise(new CircuitHasBeenInstantiatedEvent());
    }

    private GameObject GenerateElementFromSpline(String name, int width, 
        Material mat)
    {
        GameObject elementGO = new GameObject(name);
        elementGO.transform.SetParent(levelElements.transform);
        MeshFilter elementMF = elementGO.AddComponent<MeshFilter>();
        MeshRenderer elementMR = elementGO.AddComponent<MeshRenderer>();
        elementMF.sharedMesh = 
            MeshGenerator.ExtrudeMeshAlongSpline(levelBaseSpline, width);
        elementMR.material = mat;
        elementGO.AddComponent<MeshCollider>();

        return elementGO;
    }

    private List<Vector3> GenerateStartPositions()
    {
        List<Vector3> startPositions = new List<Vector3>();

        for (int i = 1; i <= (int)Math.Ceiling(numberOfOpponents / 3f); i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (startPositions.Count + 1 > numberOfOpponents + 1)
                    return startPositions;
                Vector3 newPosition = new Vector3( j * 3, 0, -i * 4);
                startPositions.Add(newPosition);
            }
        }

        return startPositions;
    }
    #endregion
}