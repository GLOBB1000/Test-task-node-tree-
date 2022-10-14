using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class MovementWithControllers : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private XRNode inputSource;

    [SerializeField] private LayerMask groundLayer;

    private XROrigin xrOrigin;

    private Vector2 inputAxis;

    private float fallingSpeed;
    private float gravity = -9.8f;

    private CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        xrOrigin = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        CharacterMovesToHeadSet();
            
        Quaternion headRotation = Quaternion.Euler(0, xrOrigin.Camera.gameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headRotation * new Vector3(inputAxis.x, 0, inputAxis.y);

        characterController.Move(direction * Time.fixedDeltaTime * speed);

        bool isGrounded = this.isGround();

        if (isGrounded)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += gravity * Time.fixedDeltaTime;
        }

        characterController.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    private void CharacterMovesToHeadSet()
    {
        characterController.height = xrOrigin.CameraInOriginSpaceHeight;
        Vector3 characterCenter = transform.InverseTransformPoint(xrOrigin.Camera.gameObject.transform.position);
        characterController.center = new Vector3(characterCenter.x,
            characterController.height / 2 + characterController.skinWidth
            , characterCenter.z);
        
    }

    bool isGround()
    {
        Vector3 rayStart = transform.TransformPoint(characterController.center);
        float rayLength = characterController.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, characterController.radius, Vector3.down, out RaycastHit info,
            rayLength, groundLayer);

        return hasHit;
    }
}

