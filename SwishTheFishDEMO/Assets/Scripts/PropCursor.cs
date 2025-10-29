using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(MeshRenderer))]
public class PropCursor : MonoBehaviour
{

    ObjectInput objectInput;
    private Camera camera;
    private MeshRenderer meshRenderer;
    ObjectInput pushProp;
    ObjectInput pullProp;
    ObjectInput spawnProp;
    ObjectInput mouseProp;
    ObjectInput diverProp;

    private School _school;
    
    private bool fishIsColliding = false;
    private float lastFishCollisionTime;

    [Tooltip("Amount of time to reset fish collision")]
    [SerializeField] private float collisionCooldown = 0.3f; 

    private void OnEnable()
    {
        EventManager.OnFishCollision += EventManagerOnOnFishCollision;
    }

    private void OnDisable()
    {
        EventManager.OnFishCollision -= EventManagerOnOnFishCollision;
    }

    private void EventManagerOnOnFishCollision(object data)
    {
        if (pullProp == null && mouseProp == null) return;
        
        fishIsColliding = true;
        lastFishCollisionTime = Time.time;
        if (meshRenderer.material.color != Color.green) meshRenderer.material.color = Color.green;
    }

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {   
            pushProp = objectInputs.TryGetValue(TableManager.PushPropId, out pushProp) ? pushProp : null;
            pullProp = objectInputs.TryGetValue(TableManager.PullPropId, out pullProp) ? pullProp : null;
            spawnProp = objectInputs.TryGetValue(TableManager.SpawnPropId, out spawnProp) ? spawnProp : null;
            mouseProp = objectInputs.TryGetValue(TableManager.MouseId, out mouseProp) ? mouseProp : null;
            diverProp = objectInputs.TryGetValue(TableManager.DiverId, out diverProp) ? diverProp : null;
            
            //Debug.Log(pullProp);
        }
        else
        {
            pushProp = null;
            pushProp = null;
            spawnProp= null;
            mouseProp = null;
            diverProp = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TableManager.Instance.OnTouch += OnTouchReceive;
        objectInput = null;
        camera = Camera.main;
        _school = FindObjectOfType<School>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastFishCollisionTime >= collisionCooldown)
        {
            fishIsColliding = false;
        }
        
        if ((pullProp != null || mouseProp != null))
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            if (!fishIsColliding) meshRenderer.material.color = Color.black;
            transform.position = pullProp != null ? Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, pullProp.position, 50)) : Helpers
                .GetWorldPositionOnPlane(camera, mouseProp.position, 50);
            if (pullProp != null)
            {
                transform.eulerAngles = new Vector3(90, Helpers.GetPropOrientationDeg(pullProp.orientation), 0);
            }
            transform.localScale = new Vector3(6, 2, 2);
        }
        else if (pushProp != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = Color.blue;
            transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, pushProp.position));
            transform.localScale = new Vector3(5, 5, 5);
        }
        else if (spawnProp != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = Color.green;
            transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, spawnProp.position));
            transform.localScale = new Vector3(5, 5, 5);
        }
        // else if (mouseProp != null)
        // {
        //     if (!meshRenderer.enabled) meshRenderer.enabled = true;
        //     meshRenderer.material.color = Color.yellow;
        //     transform.position = Helpers.GetWorldPositionOnPlane(camera, mouseProp.position, 10);
        // }
        else if (diverProp != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = Color.yellow;
            transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, diverProp.position));
            transform.localScale = new Vector3(1, 2, 1);
        }
        else if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        if ((meshRenderer && !meshRenderer.enabled) || !Application.isPlaying) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _school.PropPullDistance);
    }
}
