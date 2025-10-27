using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public class Boid : MonoBehaviour 
{
    public School School { get; set; }

    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;

    public Camera Camera;

    public float TailAmplitude = 0.2f;
    public float WiggleAmplitude = 3f;
    public float WiggleFrequency = 4f;
    private float wiggleOffset;

    public Transform TailBone;

    private void Start()
    {
        Velocity = Random.insideUnitSphere * 2;
        wiggleOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    public void UpdateSimulation(float deltaTime, List<ObjectInput> props)
    {
        //Clear acceleration from last frame
        Acceleration = Vector3.zero;
        //Apply forces
        Acceleration += (Vector3)School.GetForceFromBounds(this);
        Acceleration += GetConstraintSpeedForce();
        Acceleration += GetSteeringForce();

        foreach (var prop in props.Where(prop => prop != null))
        {
            if (prop.tagValue == TableManager.PullPropId)
            {
                Acceleration += PropPullForce(Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(Camera, prop.position)));
            }
            else if (prop.tagValue == TableManager.PushPropId)
            {
                Acceleration += PropRepelForce(Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(Camera, prop.position)));
            }
            else if (prop.tagValue == TableManager.MouseId)
            {
                Acceleration += PropPullForce(Helpers.GetWorldPositionOnPlane(Camera, prop.position));
            }
        }

        //Step simulation
        Velocity += deltaTime * Acceleration;
        Position += 0.5f * deltaTime * deltaTime * Acceleration + deltaTime * Velocity;
        
        // Body wiggle
        float wiggleAngle = Mathf.Sin(Time.time * WiggleFrequency + wiggleOffset) * WiggleAmplitude;
        if (Velocity.sqrMagnitude > 0.001f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(Velocity.normalized, Vector3.up);
            Quaternion wiggleRotation = Quaternion.AngleAxis(wiggleAngle, Vector3.up);
            transform.rotation = moveRotation * wiggleRotation;
        }

        // tail wiggle
        if (TailBone)
        {
            float tailRotationZ = Mathf.Sin(Time.time * WiggleFrequency + Mathf.PI / 4f) * TailAmplitude;
            Vector3 tailEuler = TailBone.localEulerAngles;
            tailEuler.z = tailRotationZ * Mathf.Rad2Deg; // convert radians to degrees
            TailBone.localEulerAngles = tailEuler;
        }

        transform.position = Position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") && other != null && other != GetComponent<Collider>())
        {
            Vector3 away = transform.position - other.transform.position;
            away.y = 0f; // keep on horizontal plane if desired
            float distance = away.magnitude;

            // Normalize and scale avoidance based on distance
            if (distance > 0.001f)
            {
                Vector3 avoidanceForce = away.normalized * (1f / distance) * 2f;
                Velocity += avoidanceForce * Time.deltaTime;
            }
        }
    }

    Vector3 PropPullForce(Vector3 propPosition)
    {
        var force = Vector3.zero;
        
        var distanceXZ = new Vector2(propPosition.x, propPosition.z) - new Vector2(Position.x, Position.z);
        
        if (distanceXZ.magnitude > School.PropPullDistance) return force;
        
        var distance = propPosition - Position;
        distance = new Vector3(distance.x, 0, distance.z);
        force += distance * School.PropPullForce;
            
        if (Velocity.magnitude > 0.1 && distance.magnitude < 2)
        {
            Velocity *= 0.8f;
        }

        if (distanceXZ.magnitude < School.PropVibrationMinDistance)
        {
            EventManager.Dispatch(new CustomEvent(EventManager.EventType.FishCollision, null));
        };
        
        return force;
    }

    Vector3 PropRepelForce(Vector3 propPosition)
    {
        var force = Vector3.zero;

        var distanceXZ = new Vector2(propPosition.x, propPosition.z) - new Vector2(Position.x, Position.z);

        if (distanceXZ.magnitude > School.PropRepelDistance) return force;

        var distance = propPosition - Position;
        var normDistance = distance.normalized;
        
        // done like this to prioritize horizontal movement over vertical
        var repelForceX = Random.Range(1, 4) * normDistance.x * School.PropRepelForce;
        var repelForceZ = Random.Range(1, 4) * normDistance.z * School.PropRepelForce;
        
        force -= new Vector3(repelForceX, normDistance.y, repelForceZ);
        
        return force;
    }

    Vector3 GetSteeringForce()
    {
        Vector3 cohesionForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 separationForce = Vector3.zero;
        
        float numNeighbors = 0;
        float numNeighborsCohesion = 0;
        float numNeighborsAlignment = 0;
        
        Vector3 VelocitySum = Vector3.zero;
        Vector3 PostionSum = Vector3.zero;
        
        Vector3 averageVelocity = Vector3.zero; 
        Vector3 averagePosition = Vector3.zero; 

        //Boid forces
        foreach (Boid neighbor in School.BoidManager.GetNeighbors(this, School.NeighborRadius))
        {
            float distance = (neighbor.Position - Position).magnitude;
            numNeighbors += 1;

            //Separation force
            if (distance < School.SeparationRadius)
            {
                separationForce += School.SeparationForceFactor * ((School.SeparationRadius - distance) / distance) * (Position - neighbor.Position);
            }
            
            if (distance < School.AlignmentRadius) 
            {
                if (numNeighbors > 0)
                {
                    VelocitySum += neighbor.Velocity;
                    averageVelocity = VelocitySum / numNeighbors;
                    numNeighborsCohesion += 1;
                }

            }

            if (distance < School.CohesionRadius)
            {
                if (numNeighbors > 0)
                {
                    PostionSum += neighbor.Position;
                    averagePosition = PostionSum / numNeighbors;
                    numNeighborsAlignment += 1;
                }

            }
        }
        
        //Set cohesion/alignment forces here
        if (numNeighborsAlignment > 0)
        {
         alignmentForce = School.AlignmentForceFactor*(averageVelocity - Velocity);
        }

        if (numNeighborsCohesion > 0)
        {
         cohesionForce = School.CohesionForceFactor*(averagePosition - Position);
        }



        // First calculate the average position
        // of all your neighbors within the cohesion radius, then set the force 
        // according to the given equation. Just like last time, remember to use
        // School.CohesionRadius and School.CohesionForceFactor accordingly.


        return alignmentForce + cohesionForce + separationForce;
    }

    Vector3 GetConstraintSpeedForce()
    {
        Vector3 force = Vector3.zero;

        //Apply drag
        force -= School.Drag * Velocity;

        float vel = Velocity.magnitude;
        if (vel > School.MaxSpeed)
        { 
            //If speed is above the maximum allowed speed, apply extra friction force
            force -= (20.0f * (vel - School.MaxSpeed) / vel) * Velocity;
        }
        else if (vel < School.MinSpeed)
        {
            //Increase the speed slightly in the same direction if it is below the minimum
            force += (5.0f * (School.MinSpeed - vel) / vel) * Velocity;
        }

        return force;
    }

    public Vector3 PublicGetSteeringForce()
    {
        return GetSteeringForce();
    }

    public Vector3 PublicGetConstraintSpeedForce()
    {
        return GetConstraintSpeedForce();
    }
}
