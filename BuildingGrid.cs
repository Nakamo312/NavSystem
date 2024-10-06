using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGrid : MonoBehaviour
{
    public event Action<List<Vector2Int>> placeBuild;

    public Vector2Int GridSize = new Vector2Int(10, 10);
    private Building[,] Buildings;
    private Building currentBuilding;
    private Camera mainCamera;
    private Building building;
    private PreviewSystem flyingbuilding;
    private void Awake()
    {
        Buildings = new Building[GridSize.x, GridSize.y];
        mainCamera = Camera.main;
    }

    public Building[,] buildings { get { return Buildings; } }

    public void StartPlacingBuilding(Building build)
    {
        building = build;
        if (flyingbuilding != null)
        {
            Destroy(flyingbuilding.gameObject);
        }
        flyingbuilding = Instantiate(build.preview);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flyingbuilding != null)
        {
            var Ground = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Ground.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);
                bool available = true;

                if (x > GridSize.x - flyingbuilding.Size.x || x < 0) available = false;
                if (y > GridSize.y - flyingbuilding.Size.y || y < 0) available = false;
                if (isPlaceTake(x, y)) available = false;

                flyingbuilding.SetTransparent(available);

                flyingbuilding.transform.position = new Vector3(x,0,y);
                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }

            
        }
    }
    void PlaceFlyingBuilding(int x, int y)
    {
        building = Instantiate(building);
        List<Vector2Int> cells = new();
        for (int i = 0; i < flyingbuilding.Size.x; i++)
        {
            for (int j = 0; j < flyingbuilding.Size.y; j++)
            {
                Buildings[i + x, j + y] = building;
                cells.Add(new Vector2Int(i + x, j + y));
            }
        }
        flyingbuilding.SetNormal();
        
        building.transform.position = flyingbuilding.transform.position;
        Destroy(flyingbuilding.gameObject);
        building.OnPlace();


        placeBuild?.Invoke(cells);
        //flyingbuilding = null;


    }

    bool isPlaceTake(int x, int y)
    {
        for (int i = 0; i < flyingbuilding.Size.x; i ++)
        {
            for (int j = 0; j < flyingbuilding.Size.y; j++)
            {
                if ((x + i < GridSize.x) && (y + j < GridSize.y)  && ((x + i) >= 0)  && ((y + j) >= 0))
                {
                    if (Buildings[x + i, y + j] != null) return true;
                }
                
            }
        }
        return false;
    }
}

