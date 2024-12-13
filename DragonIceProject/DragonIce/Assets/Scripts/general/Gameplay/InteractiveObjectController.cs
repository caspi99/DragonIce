using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class InteractiveObjectList
{
    public List<InteractiveObject> interactive_objects { get; private set; }
    public InteractiveObjectList(List<InteractiveObject> interactive_objects) { this.interactive_objects = interactive_objects; }
}

public class InteractiveObjectControllerLevel2 : InteractiveObjectController
{
    private GeneratorLevel2 generator;

    private List<InteractiveObjectList> interactive_object_lists;

    private List<int> num_of_interactive_objects = new List<int>();
    private string[] grabbable_placements_tags = new string[] { "StonePlacementPhase1", "TreePlacementPhase2", "StonePlacementPhase3" };
}
