using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    private List<Boid> m_boids;
    private List<Boid> to_remove;

    private School[] schools;

    ObjectInput pushProp;
    ObjectInput pullProp;
    ObjectInput spawnProp1;
    ObjectInput spawnProp2;
    ObjectInput mouseProp;

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
        to_remove = new List<Boid>();
        
        camera = Camera.main;

        schools = GameObject.FindObjectsOfType<School>();
        foreach (var school in schools)
        {
            school.BoidManager = this;
            m_boids.AddRange(school.FishSpawner());
        }

        TableManager.Instance.OnTouch += OnTouchReceive;
    }

    private void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {

            pushProp = objectInputs.TryGetValue(TableManager.PushPropId, out pushProp) ? pushProp : null;
            pullProp = objectInputs.TryGetValue(TableManager.PullPropId, out pullProp) ? pullProp : null;
            mouseProp = objectInputs.TryGetValue(TableManager.MouseId, out mouseProp) ? mouseProp : null;
            spawnProp1 = objectInputs.TryGetValue(TableManager.SpawnPropId1, out spawnProp1) ? spawnProp1 : null;
            spawnProp2 = objectInputs.TryGetValue(TableManager.SpawnPropId2, out spawnProp2) ? spawnProp2 : null;
        }

        /*
        else if (surfaceFingers.Count > 0)
        {
            foreach (KeyValuePair<int, FingerInput> entry in surfaceFingers)
            {
                pullProp = new ObjectInput(10000, 2, entry.Value.position, 0, Vector2.zero, 0, 0, 0);
            }
        }
        */

        else
        {
            pullProp = null;
            pushProp = null;
            mouseProp = null;
            spawnProp1 = null;
            spawnProp2 = null;
        }
    }

    public int GetNumBoids()
    {
        return m_boids.Count < 1 ? 0 : m_boids.Count();
    }

    private Color GetColor(string hex)
    {
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    void AddBoid(object data)
    {

        // Default random color and fish
        Color[] colors = new Color[] { Color.red, Color.blue, Color.green };
        Color fishColor = colors[Random.Range(0, colors.Length)];  // pick random color
        string fishId = Random.Range(1, 5).ToString() + Random.Range(1, 7).ToString() + Random.Range(1, 5).ToString();
        string deviceId = "0";

        if (data != null)
        {
            FishData fish = JsonUtility.FromJson<FishData>((string)data);
            fishId = fish.headId + fish.bodyId + fish.tailId;
            fishColor = GetColor(fish.color);
            deviceId = fish.deviceId;
        }

        StartCoroutine(FishSpawnAction(fishColor, fishId, deviceId));
    }

    IEnumerator FishSpawnAction(Color fishColor, string fishId, string deviceId)
    {
        if (deviceId != "0")
        {
            Debug.Log(deviceId);
            yield return new WaitForSeconds(1.0f);
            if (deviceId == "5" && spawnProp1 != null)
            {
                Vector3 spawnPos = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, spawnProp1.position));
                float angle = Helpers.GetPropOrientationDeg(spawnProp1.orientation) * Mathf.Deg2Rad;
                Vector3 spawnDir = new Vector3(-Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Debug.Log(spawnDir);

                m_boids.Add(schools[0].SpawnFish(spawnPos, spawnDir, fishColor, fishId));
            } else if (deviceId == "6" && spawnProp2 != null)
            {
                Vector3 spawnPos = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, spawnProp2.position));
                float angle = Helpers.GetPropOrientationDeg(spawnProp2.orientation) * Mathf.Deg2Rad;
                Vector3 spawnDir = new Vector3(-Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Debug.Log(spawnDir);

                m_boids.Add(schools[0].SpawnFish(spawnPos, spawnDir, fishColor, fishId));
            } else
            {
                Debug.Log("Received unknown deviceId");
            }
        } else
        {
            // Default zero location
            m_boids.Add(schools[0].SpawnFish(Vector3.zero, Vector3.zero, fishColor, fishId));
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            EventManager.Dispatch(new CustomEvent(EventManager.EventType.AddFish, null));
        }

        // Vector3 propPosition = Vector3.zero;
        // if (objectOnSurface)
        // {
        //     propPosition = Helpers.GetWorldPositionOnPlane(camera, objectInput.position);
        // }

        var props = new List<ObjectInput>
        {
            pushProp,
            pullProp,
            mouseProp,    
        };

        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime, props);
        }

        EliminateFishes();
        SwimOff();
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

    public void EliminateFishes()
    {
        int n_boids = GetNumBoids();
        int max_boids = 50;
        if (n_boids > max_boids)
        {
            int diff = n_boids - max_boids;
            for (int i = 0; i < diff; i++)
            {
                // Debug.Log("Removing a fish");
                to_remove.Add(m_boids[0]);
                m_boids.RemoveAt(0);
            }
        }
    }

    public void SwimOff()
    {
        int n_toRemove = to_remove.Count();
        if (n_toRemove < 1) return;

        float deltaTime = Time.fixedDeltaTime;

        for (int i = 0; i < n_toRemove; i++)
        {
            Boid bi = to_remove[i];

            bi.Acceleration = Vector3.zero;
            bi.Acceleration += (Vector3)bi.School.GetForceFromBounds(bi);
            bi.Acceleration += bi.PublicGetConstraintSpeedForce();
            bi.Acceleration += bi.PublicGetSteeringForce();
            bi.Acceleration *= 0.01f;
            bi.Velocity += deltaTime * bi.Acceleration;
            bi.Velocity.y = 0f;
            bi.Velocity *= 1.02f;
            bi.Position += 0.5f * deltaTime * deltaTime * bi.Acceleration + deltaTime * bi.Velocity;
        }

        int offset = 0;
        for (int i = 0; i < n_toRemove; i++)
        {
            Boid bi = to_remove[i - offset];
            if (Mathf.Abs(bi.Position.x) > 200f || Mathf.Abs(bi.Position.z) > 100f)
            {
                Destroy(bi.gameObject);
                to_remove.RemoveAt(i - offset);
                offset++;
            }
        }
    }
}
