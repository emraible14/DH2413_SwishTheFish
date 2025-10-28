using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(MeshRenderer))]
public class PropCursor : MonoBehaviour
{
    public GameObject propCursorPrefab;
    List<GameObject> propCursors;

    ObjectInput objectInput;
    private Camera camera;
    private MeshRenderer meshRenderer;
    ObjectInput pushProp;
    ObjectInput pullProp;
    ObjectInput spawnProp1;
    ObjectInput spawnProp2;
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
            spawnProp1 = objectInputs.TryGetValue(TableManager.SpawnPropId1, out spawnProp1) ? spawnProp1 : null;
            spawnProp2 = objectInputs.TryGetValue(TableManager.SpawnPropId2, out spawnProp2) ? spawnProp2 : null;
            mouseProp = objectInputs.TryGetValue(TableManager.MouseId, out mouseProp) ? mouseProp : null;
            diverProp = objectInputs.TryGetValue(TableManager.DiverId, out diverProp) ? diverProp : null;
        }
        else
        {
            pushProp = null;
            pullProp = null;
            spawnProp1 = null;
            spawnProp2 = null;
            mouseProp = null;
            diverProp = null;
        }
        //Debug.Log("the push prop is: " + pushProp);
    }

    // Start is called before the first frame update
    void Start()
    {
        TableManager.Instance.OnTouch += OnTouchReceive;
        objectInput = null;
        camera = Camera.main;
        _school = FindObjectOfType<School>();
        //meshRenderer = GetComponent<MeshRenderer>();

        propCursors = new List<GameObject>();
        List<Color> colors = new List<Color> { Color.black, Color.blue, Color.green, Color.green, Color.yellow };
        for (int i = 0; i < colors.Count; i++)
        {
            GameObject cursor_i = Instantiate(propCursorPrefab, Vector3.zero, Quaternion.identity);
            MeshRenderer mesh_i = cursor_i.GetComponent<MeshRenderer>();
            mesh_i.enabled = false;
            mesh_i.material.color = colors[i];
            propCursors.Add(cursor_i);
            Debug.Log(cursor_i);
        }
        Debug.Log(propCursors);
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
            EnableCursor(0, pullProp, new Vector3(6, 2, 2));

            GameObject pullCursor = propCursors[0].gameObject;
            MeshRenderer pullRenderer = pullCursor.GetComponent<MeshRenderer>();
            if (!fishIsColliding) pullRenderer.material.color = Color.black;
            if (pullProp != null)
            {
                pullCursor.transform.eulerAngles = new Vector3(90, Helpers.GetPropOrientationDeg(pullProp.orientation), 0);
            }
        }
        if (pushProp != null)
        {
            Debug.Log("we're showing the shark prop");
            EnableCursor(1, pushProp, new Vector3(5, 5, 5));
        }
        if (spawnProp1 != null)
        {
            EnableCursor(2, spawnProp1, new Vector3(5, 5, 5));
        }
        if (spawnProp2 != null)
        {
            EnableCursor(3, spawnProp2, new Vector3(5, 5, 5));
        }
        // else if (mouseProp != null)
        // {
        //     if (!meshRenderer.enabled) meshRenderer.enabled = true;
        //     meshRenderer.material.color = Color.yellow;
        //     transform.position = Helpers.GetWorldPositionOnPlane(camera, mouseProp.position, 10);
        // }
        if (diverProp != null)
        {
            Debug.Log("we're showing the diver cursor");
            EnableCursor(4, diverProp, new Vector3(1, 2, 1));
        }
        DisableAbsentCursors();
    }

    private void DisableAbsentCursors()
    {
        List<ObjectInput> props = new List<ObjectInput> { pushProp, pullProp, spawnProp1, spawnProp2, diverProp };
        List<int> indices = new List<int>();
        indices.AddRange(Enumerable.Range(0, props.Count));
        indices = indices.Where(i => props[i] == null).ToList();
        foreach (int i in indices)
        {
            GameObject cursor = propCursors[i].gameObject;
            MeshRenderer renderer = cursor.GetComponent<MeshRenderer>();
            if (renderer.enabled) renderer.enabled = false;
        }

    }

    private void EnableCursor(int index, ObjectInput prop, Vector3 scale)
    {
        GameObject cursor = propCursors[index].gameObject;
        MeshRenderer renderer = cursor.GetComponent<MeshRenderer>();
        if (!renderer.enabled) renderer.enabled = true;
        cursor.transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, prop.position));
        cursor.transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if ((meshRenderer && !meshRenderer.enabled) || !Application.isPlaying) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _school.PropPullDistance);
    }
}
