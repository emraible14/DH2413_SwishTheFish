using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PropCursor : MonoBehaviour
{

    ObjectInput objectInput;
    private Camera camera;
    private MeshRenderer meshRenderer;

    private School _school;

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            var matchedTagValue = false;
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
                if (entry.Value.tagValue == TableManager.MouseId || entry.Value.tagValue == TableManager.PullPropId || entry.Value.tagValue == TableManager.PushPropId)
                {
                    objectInput = entry.Value;
                    matchedTagValue = true;
                }
            }

            if (!matchedTagValue)
            {
                objectInput = null;
            }
        }
        else
        {
            objectInput = null;
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
        if (objectInput != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = TableManager.Instance.GetSurfaceObject(TableManager.PushPropId) != null
                ? Color.blue
                : Color.red;
            transform.position = Helpers.GetWorldPositionOnPlane(camera, objectInput.position);

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
