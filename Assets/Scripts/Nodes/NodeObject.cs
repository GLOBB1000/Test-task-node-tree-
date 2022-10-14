using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class NodeObject
{
    [TextArea]
    public string Name;

    [TextArea]
    public string Description;

    [SerializeField]

    private List<NodeObject> nodeObjects => NodeObjects;

    public List<NodeObject> NodeObjects;

    [HideInInspector]
    [JsonIgnore]
    public string ParentNodeName;

    public NodeObject()
    {
        
    }
}
