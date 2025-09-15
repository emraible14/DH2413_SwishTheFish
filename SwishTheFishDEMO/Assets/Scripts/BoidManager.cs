using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoidManager : MonoBehaviour
{
    private List<Boid> m_boids;

    private School[] schools;

    ObjectInput objectInput;
    bool objectOnSurface;

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        // Debug.ClearDeveloperConsole();
        if (surfaceFingers.Count > 0)
        {
            //Debug.Log(surfaceFingers.Count + " fingers:");
            foreach (KeyValuePair<int, FingerInput> entry in surfaceFingers)
            {
                //Debug.Log(entry.Key + " @ " + entry.Value.position.x + ";" + entry.Value.position.y);
            }
        }

        if (objectInputs.Count > 0)
        {
            //Debug.Log(objectInputs.Count + " objects:");
            objectOnSurface = true;
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
          
                    //Debug.Log("Setting objectInput!!!");
                    objectInput = entry.Value;
                
                //Debug.Log(entry.Key + ", tag: " + entry.Value.tagValue + " @ " + entry.Value.position.x + ";" + entry.Value.position.y);
            }
        }
        else
        {
            objectOnSurface = false;
            objectInput = null;
        }
    }

    private void OnEnable()
    {
        EventManager.OnFishAdded += AddBoid;
    }

    private void OnDisable()
    {
        EventManager.OnFishAdded -= AddBoid;
    }

    void Start()
    {
        m_boids = new List<Boid>();

        schools = GameObject.FindObjectsOfType<School>();
        foreach (var school in schools)
        {
            school.BoidManager = this;
            m_boids.AddRange(school.FishSpawner());
        }

        TableManager.Instance.OnTouch += OnTouchReceive;
        objectInput = null;
    }

    public int GetNumBoids()
    {
        return m_boids.Count < 1 ? 0 : m_boids.Count();
    }

    void AddBoid()
    {
        m_boids.Add(schools[0].SpawnFish());
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition)
    {
        var ray = Camera.main.ViewportPointToRay(screenPosition);
        var xy = new Plane(Vector3.up, new Vector3(0, 15, 0));
        xy.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Adding fish");
            AddBoid();
        }

        Vector3 propPosition = Vector3.zero;
        if (objectOnSurface)
        {
            propPosition = GetWorldPositionOnPlane(objectInput.position);
            propPosition.z *= -1;
        }
        
        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime, propPosition, objectOnSurface);
        }
    }

    public IEnumerable<Boid> GetNeighbors(Boid boid, float radius)
    {
        float radiusSq = radius * radius;
        foreach (var other in m_boids)
        {
            if (other != boid && (other.Position - boid.Position).sqrMagnitude < radiusSq)
                yield return other;
        }
    }
}
