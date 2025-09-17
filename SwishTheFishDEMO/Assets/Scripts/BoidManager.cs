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

    ObjectInput spawnProp;

    private Camera camera;

    private void OnEnable()
    {
        EventManager.OnFishAdded += AddBoid;
    }

    private void OnDisable()
    {
        EventManager.OnFishAdded -= AddBoid;
    }

    private void Start()
    {
        m_boids = new List<Boid>();
        
        camera = Camera.main;

        schools = GameObject.FindObjectsOfType<School>();
        foreach (var school in schools)
        {
            school.BoidManager = this;
            m_boids.AddRange(school.FishSpawner());
        }

        TableManager.Instance.OnTouch += OnTouchReceive;
        objectInput = null;
    }

    private void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            var matchedAnyButSpawn = false;
            var matchedSpawn = false;
            foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
            {
                if (entry.Value.tagValue == TableManager.SpawnPropId)
                {
                    spawnProp = entry.Value;
                    matchedSpawn = true;
                }
                else
                {
                    objectOnSurface = true;
                    objectInput = entry.Value;
                    matchedAnyButSpawn = true;
                }
            }

            if (!matchedAnyButSpawn)
            {
                objectInput = null;
                objectOnSurface = false;
            }

            if (!matchedSpawn)
            {
                spawnProp = null;
            }
        }
        else
        {
            objectOnSurface = false;
            objectInput = null;
            spawnProp = null;
        }
    }

   

    public int GetNumBoids()
    {
        return m_boids.Count < 1 ? 0 : m_boids.Count();
    }

    void AddBoid(object data)
    {
        var fishData = (FishData)data;
        
        if (spawnProp != null)
        {
            Vector3 spawnPos = Helpers.GetWorldPositionOnPlane(camera, spawnProp.position);
            m_boids.Add(schools[0].SpawnFish(spawnPos));

        } else
        {
            m_boids.Add(schools[0].SpawnFish(Vector3.zero));
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            EventManager.Dispatch(new CustomEvent(EventManager.EventType.AddFish, null));
        }

        Vector3 propPosition = Vector3.zero;
        if (objectOnSurface)
        {
            propPosition = Helpers.GetWorldPositionOnPlane(camera, objectInput.position);
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
