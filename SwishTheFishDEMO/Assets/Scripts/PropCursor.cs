using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCursor : MonoBehaviour
{

    ObjectInput objectInput;
    private Camera camera;
    [SerializeField] MeshRenderer meshRenderer;

    private School _school;

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            var matchedTagValue = false;
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
                if (entry.Value.tagValue == TableManager.MouseId || entry.Value.tagValue == TableManager.PullPropId)
                {
                    Debug.Log("Matched");
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
    }

    // Update is called once per frame
    void Update()
    {
        if (objectInput != null)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            transform.position = Helpers.GetWorldPositionOnPlane(camera, objectInput.position);

        }
        else if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!meshRenderer.enabled) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _school.PropPullDistance);
    }
}
