using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class BuildingLogic
{
    [SerializeField]
    int max_hp;
    private byte level;
    protected PlayerManager player;

    public BuildingLogic(int _max_hp, byte _level, PlayerManager _player)
    {
        max_hp = _max_hp;
        level = _level;
        player = _player;
    }
    virtual public void Update() 
    {

    }
}

public class GeneratorLogic : BuildingLogic
{
    protected int productivity;
    private float productionTime;
    protected Resource resource;

    public GeneratorLogic(int _max_hp, byte _level,
                PlayerManager _player,
                float _productionTime,
                int _productivity, Resource _resource) : base(_max_hp, _level, _player)
    {
        productivity = _productivity;
        productionTime = _productionTime;
        resource = _resource;
    }
    override public void Update()
    {
        player.UpdateResourse(resource.id, productivity);
    }
}

public class FabricLogic : GeneratorLogic
{
    private Dictionary<int, int> resourcesIn;
    public FabricLogic(int _max_hp, byte _level,
                PlayerManager _player,
                float _productionTime,
                int _productivity, Resource _resource, Dictionary<int, int> _resourcesIn) : base(_max_hp, _level, _player, _productionTime, _productivity, _resource)
    {
        resourcesIn = _resourcesIn;
    }
    override public void Update()
    {
        bool isUpdate = true;
        foreach (KeyValuePair<int,int> pair in resourcesIn)
        {
            if (!player.UpdateResourse(pair.Key, -pair.Value))
            {
                isUpdate = false;
                break;
            }
        }
        if(isUpdate)
        {
            player.UpdateResourse(resource.id, productivity);
            Debug.Log("+1");
        }
    }
}

public class Building : MonoBehaviour
{
    public PlayerManager player;
    public Vector2Int Size = Vector2Int.one;
    [SerializeField] public PreviewSystem preview;
    public Build info;
    [SerializeField] const int level_count = 4;
    [SerializeField] private int productivity;
    [SerializeField] private float productionTime;
    [SerializeField] private float interval;
    [SerializeField] private Resource resource;
    [SerializeField] public Dictionary<int, int> resourcesIn = new Dictionary<int, int>();
    [SerializeField] List<int> hp_levels = new List<int>(level_count);
    private BuildingLogic logic;

    void Start()
    {

        resourcesIn.Add(0, 10);
    }

    public bool OnPlace()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<PlayerManager>();
        switch (info.type)
        {
            case BuildType.Generator:
                {
                    logic = new GeneratorLogic(hp_levels[0], 1, player, productionTime, productivity, resource);
                    return true;
                }
            case BuildType.Factory:
                {
                    logic = new FabricLogic(hp_levels[0], 1, player, productionTime, productivity, resource, resourcesIn);
                    return true;
                }
            default:
                {
                    return false;
                }
        }
        
    }
    private void OnDrawGizmos()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Gizmos.color = new Color(1f, 0.68f, 1f, 0.3f);
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, 0.1f, 1));
            }
        }
    }

    public void Select()
    {
        EventBus.onSelected?.Invoke(info);
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (logic != null)
        {
            productionTime += Time.deltaTime;
            if (productionTime >= interval)
            {
                logic.Update();
                productionTime = 0;
            }
            
        }

    }
}
