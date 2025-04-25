using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    [Header("Generators")] [SerializeField]
    private GenerateObject[] ChikenGenerators;

    [SerializeField] private GenerateObject catGenerator;
    [SerializeField] private GenerateObject dogGenerator;

    [Space(10)] [Header("Parenting and Mutation")] [SerializeField]
    private float mutationFactor;
    [SerializeField] private float mutationChance;
    [SerializeField] private int catParentSize;
    [SerializeField] private int dogParentSize;

    [Space(10)] [Header("Simulation Controls")] [SerializeField, Tooltip("Time per simulation (in seconds).")]
    private float simulationTimer;

    [SerializeField, Tooltip("Current time spent on this simulation.")]
    private float simulationCount;

    [SerializeField, Tooltip("Automatically starts the simulation on Play.")]
    private bool runOnStart;

    [SerializeField, Tooltip("Let Dogs Evolute.")]
    private bool dogEvolution;

    [SerializeField, Tooltip("Let Cats Evolute.")]
    private bool catEvolution;

    [SerializeField, Tooltip("Initial count for the simulation. Used for the Prefabs naming.")]
    private int generationCount;

    [Space(10)] [Header("Prefab Saving")] [SerializeField]
    private string savePrefabsAt;

    [Header("Former winners")] [SerializeField]
    private AgentData lastcatWinnerData;

    [SerializeField] private AgentData lastdogWinnerData;

    private bool _runningSimulation;
    private List<CatLogic> _activecats;
    private List<DogLogic> _activedogs;
    private CatLogic[] _catParents;
    private DogLogic[] _dogParents;

    private void Awake()
    {
        Random.InitState(6);
    }

    private void Start()
    {
        if (runOnStart)
        {
            StartSimulation();
        }
    }

    private void Update()
    {
        if (!_runningSimulation) return;
        //Creates a new generation.
        if (simulationCount >= simulationTimer)
        {
            ++generationCount;
            MakeNewGeneration();
            simulationCount = simulationCount - simulationTimer;
        }

        simulationCount += Time.deltaTime;
    }

    public void GenerateChiken()
    {
        foreach (var GenerateObject in ChikenGenerators)
        {
            GenerateObject.RegenerateObjects();
        }
    }

    public void GenerateObjects(CatLogic[] catParents = null, DogLogic[] dogParents = null)
    {
        Generatecats(catParents);
        Generatedogs(dogParents);
    }


    private void Generatedogs(DogLogic[] dogParents)
    {
        _activedogs = new List<DogLogic>();
        var objects = dogGenerator.RegenerateObjects();
        foreach (var dog in objects.Select(obj => obj.GetComponent<DogLogic>()).Where(dog => dog != null))
        {
            _activedogs.Add(dog);
            if (dogParents != null)
            {
                var dogParent = dogParents[Random.Range(0, dogParents.Length)];
                dog.Birth(dogParent.GetData());
            }
            if(dogEvolution){
            dog.Mutate(mutationFactor, mutationChance);
            }
            dog.AwakeUp();
        }
    }



    private void Generatecats(CatLogic[] catParents)
    {
        _activecats = new List<CatLogic>();
        var objects = catGenerator.RegenerateObjects();
        foreach (var cat in objects.Select(obj => obj.GetComponent<CatLogic>()).Where(cat => cat != null))
        {
            _activecats.Add(cat);
            if (catParents != null)
            {
                var catParent = catParents[Random.Range(0, catParents.Length)];
                cat.Birth(catParent.GetData());
            }
            if(catEvolution){
            cat.Mutate(mutationFactor, mutationChance);
            }
            cat.AwakeUp();
        }
    }

    private void MakeNewGeneration()
    {
        Random.InitState(6);

        GenerateChiken();

        //Fetch parents
        _activecats.RemoveAll(item => item == null);
        _activecats.Sort();
        if (_activecats.Count == 0)
        {
            Generatecats(_catParents);
        }

        _catParents = new CatLogic[catParentSize];
        for (var i = 0; i < catParentSize; i++)
        {
            _catParents[i] = _activecats[i];
        }

        var lastcatWinner = _activecats[0];
        lastcatWinner.name += "Gen-" + generationCount;
        lastcatWinnerData = lastcatWinner.GetData();
        PrefabUtility.SaveAsPrefabAsset(lastcatWinner.gameObject, savePrefabsAt + lastcatWinner.name + ".prefab");

        _activedogs.RemoveAll(item => item == null);
        _activedogs.Sort();
        _dogParents = new DogLogic[dogParentSize];
        for (var i = 0; i < dogParentSize; i++)
        {
            _dogParents[i] = _activedogs[i];
        }

        var lastdogWinner = _activedogs[0];
        lastdogWinner.name += "Gen-" + generationCount;
        lastdogWinnerData = lastdogWinner.GetData();
        PrefabUtility.SaveAsPrefabAsset(lastdogWinner.gameObject, savePrefabsAt + lastdogWinner.name + ".prefab");

        //Winners:
        Debug.Log("Last winner cat had: " + lastcatWinner.GetPoints() + " points!" + " Last winner dog had: " +
                  lastdogWinner.GetPoints() + " points!");

        GenerateObjects(_catParents, _dogParents);
    }

    public void StartSimulation()
    {
        Random.InitState(6);

        GenerateChiken();
        GenerateObjects();
        _runningSimulation = true;
    }

 
    public void ContinueSimulation()
    {
        MakeNewGeneration();
        _runningSimulation = true;
    }

    public void StopSimulation()
    {
        _runningSimulation = false;
        _activecats.RemoveAll(item => item == null);
        _activecats.ForEach(cat => cat.Sleep());
        _activedogs.ForEach(dog => dog.Sleep());
    }
}