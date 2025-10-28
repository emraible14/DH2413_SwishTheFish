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

    [Tooltip("Multiplicative force to the fish acceleration away from the repel prop")]
    [SerializeField] private float propRepelForce = 1f;
    public float PropRepelForce => propRepelForce;
    
    [Tooltip("Min distance to prop for vibration activation")]
    [SerializeField] private float propVibrationMinDistance = 0.2f;
    public float PropVibrationMinDistance => propVibrationMinDistance;



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
            // random spawning
            Color[] colors = new Color[] { Color.red, Color.blue, Color.green };
            Color fishColor = colors[Random.Range(0, colors.Length)];  // pick random color
            string fishId = Random.Range(1, 5).ToString() + Random.Range(1, 7).ToString() + Random.Range(1, 5).ToString();

            var boid = SpawnFish(Vector3.zero, Vector3.zero, fishColor, fishId);
            yield return boid;
        }
    }


    private Boid CreateFishObject(Vector3 spawnPoint, Color fishColor, string fishId)
    {
        // Load correct resource based on fishId and add scripts
        var loadedResource = Resources.Load<GameObject>("fish" + fishId);
        var temp = Instantiate(loadedResource, spawnPoint, Quaternion.identity, this.transform);
        temp.AddComponent<Boid>();
        temp.AddComponent<Fish>();

        Boid boid = temp.GetComponent<Boid>();

        // update fish color
        SkinnedMeshRenderer[] renderers = boid.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.name.Substring(0, 3) != "Mat")
                {
                    mat.color = fishColor;
                }
            }
        }

        SkinnedMeshRenderer rend = boid.GetComponentInChildren<SkinnedMeshRenderer>();
        for (int i = 2; i < rend.materials.Length; i++)
        {
            Material mat = rend.materials[i];
            mat.color = fishColor;
        }
        //rend.materials[1].color = Color.black;

        return boid;
    }

    public Boid SpawnFish(Vector3 spawnPos, Vector3 spawnDir, Color fishColor, string fishId)
    {
        Vector3 spawnPoint = spawnPos == Vector3.zero ? transform.position + m_spawnRadius * Random.insideUnitSphere : spawnPos;

        for (int j = 0; j < 3; ++j)
            spawnPoint[j] = Mathf.Clamp(spawnPoint[j], m_bounds.bounds.min[j], m_bounds.bounds.max[j]);

        Boid boid = CreateFishObject(spawnPoint, fishColor, fishId);

        // add rigidbody
        Rigidbody rb = boid.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            rb = boid.gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // add collider 
        CapsuleCollider collider = boid.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = boid.gameObject.AddComponent<CapsuleCollider>();
        }
        collider.direction = 2; // along Z axis
        collider.isTrigger = true;

        Renderer rend = boid.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Vector3 localSize = rend.bounds.size;
            Vector3 localCenter = boid.transform.InverseTransformPoint(rend.bounds.center);

            // Fit capsule roughly to the length and thickness of the mesh
            collider.height = localSize.z + 2.0f;          // along forward axis, with a little extra padding
            collider.radius = Mathf.Max(localSize.x, localSize.y) * 0.5f;  // half width/height
            collider.center = localCenter;
        }
        else
        {
            // Defaults
            collider.radius = 2.5f;
            collider.height = 15.0f;
            collider.center = Vector3.zero;
        }

        // set position and velocity
        boid.Position = spawnPoint;
        Vector3 initialDirection = spawnDir == Vector3.zero ? Vector3.forward : spawnDir;
        boid.Velocity = initialDirection * MinSpeed;
        boid.transform.rotation = Quaternion.LookRotation(initialDirection);

        // make smaller
        boid.transform.localScale *= 0.8f; 

        // set tag, school, camera, tailbone
        boid.tag = "Fish";
        boid.gameObject.layer = LayerMask.NameToLayer("Fish");
        boid.School = this;
        boid.transform.parent = this.transform;
        boid.Camera = _camera;
        boid.TailBone = boid.transform.Find("Armature/tail");

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, m_bounds.size);
    }
}
