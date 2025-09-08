using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoidManager : MonoBehaviour
{
    private List<Boid> m_boids;

    private School[] schools;

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
    }

    public int GetNumBoids()
    {
        return m_boids.Count < 1 ? 0 : m_boids.Count();
    }

    void AddBoid()
    {
        m_boids.Add(schools[0].SpawnFish());
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Adding fish");
            AddBoid();
        }
        
        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime);
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
