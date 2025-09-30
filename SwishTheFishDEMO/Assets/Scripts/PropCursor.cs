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
    ObjectInput pushProp;
    ObjectInput pullProp;

    private School _school;

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
                if (entry.Value.tagValue == TableManager.PushPropId)
                {
                    pushProp = entry.Value;
                }
                else
                {
                    pushProp = null;
                }

                if (entry.Value.tagValue == TableManager.PullPropId)
                {
                    pullProp = entry.Value;
                }
                else
                {
                    pullProp = null;
                }
            }
        }
        else
        {
            pushProp = null;
            pullProp = null;
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
        
        if (pullProp != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = Color.red;
            transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, pullProp.position));
            transform.eulerAngles = new Vector3(90, Helpers.GetPropOrientationDeg(pullProp.orientation), 0);

        }
        else if (pushProp != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            meshRenderer.material.color = Color.blue;
            transform.position = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, pushProp.position));

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
