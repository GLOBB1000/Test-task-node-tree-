using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Node;

public class TwoHandsController : MonoBehaviour
{
    [SerializeField]
    private XRController xRController;
    [SerializeField]
    private XRController xLController;

    [SerializeField]
    private Transform movementPivot;

    [SerializeField]
    private Vector3 offset;

    private NodeCreator nodeCreator;

    private NodeGameobject targetNode;

    private Vector3 startScale = new Vector3();
    private float initialDistance = 0;

    private Vector3 startHandsPosition = new Vector3();

    private bool nodeFound = false;

    void Awake()
    {
        nodeCreator = FindObjectOfType<NodeCreator>();
        nodeCreator.OnNodeCreated += NodeCreator_OnNodeCreated;
    }

    private void NodeCreator_OnNodeCreated(NodeGameobject obj)
    {
        targetNode = obj;
        nodeFound = true;
    }

    void Update()
    {
        Debug.Log("Node found " + nodeFound);
        if (!nodeFound)
            return;

        if (IsGripButton(xRController.inputDevice) && IsGripButton(xLController.inputDevice))
        {
            var distance = Vector3.Distance(xRController.transform.position, xLController.transform.position);
            var factor = distance / initialDistance;

            targetNode.transform.localScale = startScale * factor;

            targetNode.transform.parent = movementPivot;

            var centerHandPosition = (xRController.transform.position + xLController.transform.position) / 2;

            targetNode.transform.position = centerHandPosition;
        }

        else
        {
            startScale = targetNode.transform.localScale;
            initialDistance = Vector3.Distance(xRController.transform.position, xLController.transform.position);

            startHandsPosition = (xRController.transform.position + xLController.transform.position) / 2;
            targetNode.transform.parent = null;
        }
    }

    private bool IsGripButton(InputDevice inputDevice)
    {
        inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var gripValue);
        return gripValue;
    }
}
