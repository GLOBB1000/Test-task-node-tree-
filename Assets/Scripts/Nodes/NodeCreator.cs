using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Node
{
    public class NodeCreator : MonoBehaviour
    {
        private List<NodeObject> nodes = new List<NodeObject>();

        [SerializeField]
        private GameObject nodePrefab;

        [SerializeField]
        private List<string> nodeGameobjects;

        [SerializeField]
        private float decreaseRadius;

        public event Action<NodeGameobject> OnNodeCreated;

        // Start is called before the first frame update
        void Start()
        {
            DeserializeJson();
        }
        private void DeserializeJson()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Config.json");
            var json = File.ReadAllText(path);
            nodes = JsonConvert.DeserializeObject<List<NodeObject>>(json);

            FullDesirialization(nodes);

            SetNodeTree(decreaseRadius);
        }

        private void FullDesirialization(List<NodeObject> nodeObjects, Transform parent = null)
        {
            foreach (var node in nodeObjects)
            {
                if (node.NodeObjects != null && node.NodeObjects.Count > 0)
                {
                    Transform parentObj = null;

                    if (!nodeGameobjects.Contains(node.Name))
                    {
                        nodeGameobjects.Add(node.Name);
                        var obj = CreateNodeGameObject(node, parent);
                        parentObj = obj.transform;
                    }
                    else
                        parentObj = parent;

                    foreach (var nodeObject in node.NodeObjects)
                    {
                        if (nodeObject.NodeObjects != null && nodeObject.NodeObjects.Count > 0)
                            FullDesirialization(node.NodeObjects, parentObj);
                        else
                        {
                            if (!nodeGameobjects.Exists(x => x.Contains(nodeObject.Name)))
                            {
                                nodeGameobjects.Add(nodeObject.Name);
                                var obj = CreateNodeGameObject(nodeObject, parentObj);
                            }
                        }
                    }
                }
            }
        }

        private NodeGameobject CreateNodeGameObject(NodeObject node, Transform parent = null)
        {
            var gameObject = Instantiate(nodePrefab);

            var nodeObj = gameObject.GetComponent<NodeGameobject>();

            nodeObj.Initialize(node, parent);

            return nodeObj;
        }

        private void SetNodeTree(float rad)
        {
            var nodes = FindObjectsOfType<NodeGameobject>();

            nodes[nodes.Length - 1].SetChildNeededPos(rad);
            nodes[nodes.Length - 1].transform.localScale = Vector3.one * 0.1f;

            OnNodeCreated?.Invoke(nodes[nodes.Length - 1]);
        }
    }
}

