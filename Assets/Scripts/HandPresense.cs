using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresense : MonoBehaviour
{
    [SerializeField] private InputDeviceCharacteristics inputDeviceCharacteristics;
    [SerializeField] private bool isControllerShown;
    
    public InputDevice targetDevice { get; private set; }

    [SerializeField] private List<GameObject> controllerPrefabs;

    [SerializeField] private GameObject handPrefab;

    private GameObject spawnedHand;

    private GameObject spawnedController;
    private Animator handAnimator;
    void Start()
    {
        TryToInitialize();
    }

    private void TryToInitialize()
    {
        List<InputDevice> inputDevicesList = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, inputDevicesList);

        foreach (var device in inputDevicesList)
        {
            
        }

        if (inputDevicesList.Count > 0)
        {
            targetDevice = inputDevicesList[0];

            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }

            spawnedHand = Instantiate(handPrefab, transform);

            handAnimator = spawnedHand.GetComponent<Animator>();
        }
    }

    private void UpdateHandAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }

        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }

        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetDevice.isValid)
        {
            TryToInitialize();
        }

        else
        {
            if (isControllerShown)
            {
                spawnedController.SetActive(true);
                spawnedHand.SetActive(false);
            }

            else
            {
                spawnedController.SetActive(false);
                spawnedHand.SetActive(true);
                UpdateHandAnimation();
            }
        }
    }
}
