using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class BoidManager : MonoBehaviour
{
    private List<Boid> m_boids;

    private School[] schools;

    ObjectInput pushProp;
    ObjectInput pullProp;
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
    }

     private void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
     {
         if (objectInputs.Count > 0)
         {
             foreach (KeyValuePair<int, ObjectInput> entry in objectInputs)
             {
                 if (entry.Value.tagValue == TableManager.SpawnPropId)
                 {
                    spawnProp = entry.Value;
                 }
                 else
                 {
                    spawnProp = null;
                 }

                if (entry.Value.tagValue == TableManager.PushPropId)
                {
                    pushProp = entry.Value;
                    Debug.Log(Helpers.GetPropOrientationDeg(pushProp.orientation));
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
    
             //if (!matchedAnyButSpawn)
             //{
             //   objectInput = null;
             //    objectOnSurface = false;
             //}
    
             //if (!matchedSpawn)
             //{
             //    spawnProp = null;
             //}
         }
         else
         {
            pullProp = null;
            pushProp = null;
            spawnProp = null;
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

        // Default color and fish
        Color fishColor = Color.red;
        string fishId = "112";

        if (data != null)
        {
            Debug.Log((string)data);
            FishData fish = JsonUtility.FromJson<FishData>((string)data);
            fishId = fish.headId + fish.bodyId + fish.tailId;
            fishColor = GetColor(fish.color);
        }
        
        if (spawnProp != null)
        {
            Vector3 spawnPos = Helpers.GetWorldPositionOnPlane(camera, spawnProp.position);
            m_boids.Add(schools[0].SpawnFish(spawnPos, fishColor, fishId));

        }
        else
        {
            // Default zero location
            m_boids.Add(schools[0].SpawnFish(Vector3.zero, fishColor, fishId));
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
            pullProp    
        };

        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime, props);
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
