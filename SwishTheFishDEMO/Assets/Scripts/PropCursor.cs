using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCursor : MonoBehaviour
{

    ObjectInput objectInput;
    [SerializeField] MeshRenderer meshRenderer;

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            //Debug.Log(objectInputs.Count + " objects:");
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
                if (entry.Value.tagValue != 4)
                {
                    objectInput = entry.Value;
                }

                //Debug.Log(entry.Key + ", tag: " + entry.Value.tagValue + " @ " + entry.Value.position.x + ";" + entry.Value.position.y);
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

    }
    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition)
    {
        var ray = Camera.main.ViewportPointToRay(screenPosition);
        var xy = new Plane(Vector3.up, new Vector3(0, 0, 0));
        xy.Raycast(ray, out var distance);
        var point = ray.GetPoint(distance);
        point.z *= -1;
        return point;
    }
    // Update is called once per frame
    void Update()
    {
        if (objectInput != null)
        {
            Debug.Log(objectInput.position);
            meshRenderer.enabled = true;
            transform.position = GetWorldPositionOnPlane(objectInput.position);

        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
