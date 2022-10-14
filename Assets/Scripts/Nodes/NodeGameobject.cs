using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Node
{
    public class NodeGameobject : MonoBehaviour
    {
        private string nodeName;
        private string description;

        private float angleOffset = 0;
        private float radIncrease = 15;

        public NodeObject nodeObject { get; private set; }

        [SerializeField]
        private CustomLineRenderer lineRendererPrefab;

        [SerializeField]
        private DescriptionPanel descriptionPanel;

        public void Initialize(NodeObject node, Transform parentObj = null)
        {
            nodeObject = node;

            nodeName = node.Name;

            var split = node.Name.LastIndexOf('/');
            var splited = node.Name.Substring(split + 1);
            nodeName = splited;


            gameObject.name = nodeName;

            description = node.Description;

            Debug.Log($"Name: {node.Name}" +
                $"\nDecription: {node.Description}");

            if (parentObj != null)
            {
                transform.parent = parentObj;
            }

            gameObject.GetComponentInChildren<TextMeshPro>().text = nodeName;

            descriptionPanel.Inintialize(node.Description);
        }

        public void SetChildNeededPos(float radius)
        {
            if (this.nodeObject.NodeObjects != null)
            {
                angleOffset = (150 - radius) / (transform.childCount - 1);
            }

            var increasedPos = angleOffset;
            radius -= 5;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var childNode = child.GetComponent<NodeGameobject>();

                if (childNode != null && this.nodeObject.NodeObjects != null)
                {
                    SetToRadius(Vector3.zero, increasedPos, radius, child.transform);

                    childNode.SetChildNeededPos(radius);

                    var go = Instantiate(lineRendererPrefab.gameObject, childNode.transform);
                    var line = go.GetComponent<CustomLineRenderer>();
                    line.SetUpLine(new Transform[] { transform, childNode.transform });
                }
                increasedPos += angleOffset;
            }
        }

        private void SetToRadius(Vector3 center, float nextAngle, float radius, Transform childPos)
        {
            var x = center.x + radius * Mathf.Cos(nextAngle * 1.6f * Mathf.Deg2Rad);
            var y = center.y + radius * Mathf.Sin(-nextAngle * 1.6f * Mathf.Deg2Rad);

            Vector2 pos = new Vector2(x, y);
            childPos.localPosition = pos;
        }

        private void Update()
        {
            SetCollisionDetection();
        }

        private void SetCollisionDetection()
        {
            var size = Physics.OverlapSphere(transform.position, transform.lossyScale.x);

            foreach (var hitCollider in size)
            {
                XRController obj = hitCollider.GetComponent<XRController>();
                if (obj != null)
                {
                    Debug.Log(obj.name);
                    obj.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerValue);
                    if (triggerValue)
                        descriptionPanel.gameObject.SetActive(true);
                    else
                        descriptionPanel.gameObject.SetActive(false);
                }
            }
        }

    }
}