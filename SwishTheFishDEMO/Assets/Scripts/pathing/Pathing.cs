using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathing : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Gizmo")]
    [SerializeField]
    protected Color GizmoColor = Color.green;
#endif
    [SerializeField, HideInInspector]
    private GameObject waypointParent;
    [Header("Waypoints")]
    public List<Transform> Points = new List<Transform>();

    [SerializeField]
    private int startingPointIndex;
    
    [SerializeField] bool looped = false;
    
    protected int CurrentPointIndex = 0;

    public void Awake()
    {
        // If there are no waypoints, add one.
        // if (this.Points.Count == 0)
        // {
        //     this.AddWaypoint();
        // }

        this.startingPointIndex = Mathf.Clamp(this.startingPointIndex, 0, this.Points.Count - 1);

        this.CurrentPointIndex = this.startingPointIndex;
    }

    public void SetStartingPoint(int index)
    {
        this.CurrentPointIndex = Mathf.Clamp(index, 0, this.Points.Count - 1);
    }

    public Transform Walk()
    {
        var nextPoint = this.Points[this.CurrentPointIndex];
        
        if (looped)
        {
            CurrentPointIndex = (CurrentPointIndex + 1) % this.Points.Count;
        }
        else
        {
            CurrentPointIndex = Mathf.Clamp(this.CurrentPointIndex + 1, 0, this.Points.Count - 1);
        }
        
        return nextPoint;
    }


#if UNITY_EDITOR
    /// <summary>
    /// Adds a waypoint to the platform.
    /// </summary>
    public void AddWaypoint()
    {
        if (this.waypointParent == null)
        {
            this.waypointParent = new GameObject();
            this.waypointParent.transform.parent = this.transform;
            this.waypointParent.transform.position = this.transform.position;
            this.waypointParent.name = "Waypoints";
        }

        var newPoint = new GameObject();

        newPoint.transform.parent = this.waypointParent.transform;
        newPoint.transform.position = this.waypointParent.transform.position;

        // if (this.Points.Count > 1)
        // {
        //     newPoint.transform.position = this.Points[^1].transform.position + (this.Points[^1].transform.position - this.Points[^2].transform.position);
        // }

        newPoint.name = "Waypoint " + this.Points.Count.ToString();

        this.Points.Add(newPoint.transform);
    }

    /// <summary>
    /// Removes the last waypoint from the platform.
    /// </summary>
    public void SubtractWaypoint()
    {
        if (this.Points.Count == 0)
        {
            return;
        }

        var lastPoint = this.Points.Last();
        this.Points.RemoveAt(this.Points.Count - 1);
        DestroyImmediate(lastPoint.gameObject);
    }

    /// <summary>
    /// Removes all waypoints from the platform.
    /// </summary> 
    public void DeleteAllWaypoints()
    {
        this.Points.Clear();

        var children = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.name == "Waypoints")
            {
                children.Add(child.gameObject);
            }
        }
        children.ForEach(child => DestroyImmediate(child));
    }

    public void OnDrawGizmos()
    {
        if (this.Points.Count == 0)
        {
            return;
        }

        Gizmos.color = this.GizmoColor;

        var previousePoint = this.Points[0];

        for (var i = 0; i < this.Points.Count; i++)
        {
            if (i == 0)
            {
                Gizmos.DrawSphere(this.Points[i].position, 0.3f);
                continue;
            }
            Gizmos.DrawLine(previousePoint.position, this.Points[i].position);
            Gizmos.DrawSphere(this.Points[i].position, 0.3f);
            previousePoint = this.Points[i];
        }

        if (looped)
        {
            Gizmos.DrawLine(previousePoint.position, this.Points[0].position);
        }
    }
#endif
}
