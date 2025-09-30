using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class School : MonoBehaviour
{
    private Camera _camera;
    
    [SerializeField]
    private int m_numFish = 50;

    public int GetNumFish()
    {
        return m_numFish;
    }

    [SerializeField]
    private Boid m_fishPrefab = null;

    [SerializeField]
    private float m_spawnRadius = 10;

    [SerializeField]
    private BoxCollider m_bounds = null;

    [SerializeField]
    private float m_boundsForceFactor = 5;

    [Header("Prop interactions")] 
    [SerializeField] private float propPullDistance = 10f;
    [Tooltip("Multiplicative force to the fish acceleration towards the pull prop")]
    [SerializeField] private float propPullForce = 1f;

    public float PropPullForce => propPullForce;
    public float PropPullDistance => propPullDistance;

    [SerializeField] private float propRepelDistance = 10f;
    [SerializeField] private float propRepelForce = 1f;
    public float PropRepelForce => propRepelForce;



    public float PropRepelDistance
    {
        get { return propRepelDistance; }
    }

    [Header("Boid behaviour data. Experiment with changing these during runtime")]
    [SerializeField]
    private float m_cohesionForceFactor = 1;
    public float CohesionForceFactor
    {
        get { return m_cohesionForceFactor; }
        set { m_cohesionForceFactor = value; }
    }

    [SerializeField]
    private float m_cohesionRadius = 3;
    public float CohesionRadius
    {
        get { return m_cohesionRadius; }
        set { m_cohesionRadius = value; }
    }

    [SerializeField]
    private float m_separationForceFactor = 1;
    public float SeparationForceFactor
    {
        get { return m_separationForceFactor; }
        set { m_separationForceFactor = value; }
    }

    [SerializeField]
    private float m_separationRadius = 2;
    public float SeparationRadius
    {
        get { return m_separationRadius; }
        set { m_separationRadius = value; }
    }

    [SerializeField]
    private float m_alignmentForceFactor = 1;
    public float AlignmentForceFactor
    {
        get { return m_alignmentForceFactor; }
        set { m_alignmentForceFactor = value; }
    }

    [SerializeField]
    private float m_alignmentRadius = 3;
    public float AlignmentRadius
    {
        get { return m_alignmentRadius; }
        set { m_alignmentRadius = value; }
    }

    [SerializeField]
    private float m_maxSpeed = 8;
    public float MaxSpeed
    {
        get { return m_maxSpeed; }
        set { m_maxSpeed = value; }
    }

    [SerializeField]
    private float m_minSpeed;
    public float MinSpeed
    {
        get { return m_minSpeed; }
        set { m_minSpeed = value; }
    }

    [SerializeField]
    private float m_drag = 0.1f;
    public float Drag
    {
        get { return m_drag; }
        set { m_drag = value; }
    }

    public float NeighborRadius
    {
        get { return Mathf.Max(m_alignmentRadius, Mathf.Max(m_separationRadius, m_cohesionRadius)); }
    }

    public BoidManager BoidManager { get; set; }

    private void Awake()
    {
        _camera = Camera.main;
    }

    public IEnumerable<Boid> FishSpawner()
    {
        for (int i = 0; i < m_numFish; ++i)
        {
            var boid = SpawnFish(Vector3.zero);
            yield return boid;
        }
    }

    public Boid SpawnFish(Vector3 spawnPos)
    {
        
        Vector3 spawnPoint = spawnPos == Vector3.zero ? transform.position + m_spawnRadius * Random.insideUnitSphere : spawnPos;

        for (int j = 0; j < 3; ++j)
            spawnPoint[j] = Mathf.Clamp(spawnPoint[j], m_bounds.bounds.min[j], m_bounds.bounds.max[j]);
        
        Boid boid = Instantiate(m_fishPrefab, spawnPoint, m_fishPrefab.transform.rotation);
        boid.Position = spawnPoint;
        boid.Velocity = Random.insideUnitSphere;
        boid.School = this;
        boid.transform.parent = this.transform;
        boid.Camera = _camera;
        return boid;
    }

    public Vector3 GetForceFromBounds(Boid boid)
    {
        Vector3 force = new Vector3();
        Vector3 centerToPos = (Vector3)boid.Position - transform.position;
        Vector3 minDiff = centerToPos + m_bounds.size * 0.5f;
        Vector3 maxDiff = centerToPos - m_bounds.size * 0.5f;
        float friction = 0.0f;

        for (int i = 0; i < 3; ++i)
        {
            if (minDiff[i] < 0)
                force[i] = minDiff[i];
            else if (maxDiff[i] > 0)
                force[i] = maxDiff[i];
            else
                force[i] = 0;

            friction += Mathf.Abs(force[i]);
        }

        force += 0.1f * friction * (Vector3)boid.Velocity;
        return -m_boundsForceFactor * force;
    }

    void  OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, m_bounds.size);
    }
}
