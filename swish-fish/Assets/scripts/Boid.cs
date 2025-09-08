using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour 
{
    public School School { get; set; }

    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;
    
    void Start()
    {
        Velocity = Random.insideUnitSphere * 2;
    }

    public void UpdateSimulation(float deltaTime)
    {
        //Clear acceleration from last frame
        Acceleration = Vector3.zero;
            //Apply forces
        Acceleration += (Vector3)School.GetForceFromBounds(this);
        Acceleration += GetConstraintSpeedForce();
        Acceleration += GetSteeringForce();
        Acceleration += MousePullForce() * 2;
            

        //Step simulation
        Velocity += deltaTime * Acceleration;
        Position +=  0.5f * deltaTime * deltaTime * Acceleration + deltaTime * Velocity;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        var xy = new Plane(Vector3.up, new Vector3(0, 15, 0));
        xy.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    Vector3 MousePullForce()
    {
        var force = Vector3.zero;
        
        if (!Input.GetKey(KeyCode.Mouse0)) return force;
        
        var mousePos = GetWorldPositionOnPlane(Input.mousePosition, 0);
        var distance = mousePos - Position; 
        force += distance;
            
        if (Velocity.magnitude > 0.1 && distance.magnitude < 2)
        {
            Velocity *= 0.8f;
        }


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

    // private Vector3 ClampToViewPortBounds(Vector3 vector)
    // {
    //     // var viewportPos = _camera.WorldToViewportPoint(vector);
    //     //
    //     // if (viewportPos.x > 1)
    //     // {
    //     //     viewportPos.x = 0;
    //     // }
    //     // else if (viewportPos.x < 0)
    //     // {
    //     //     viewportPos.x = 1;
    //     // }
    //     //
    //     // if (viewportPos.y > 1)
    //     // {
    //     //     viewportPos.y = 0;
    //     // }
    //     // else if (viewportPos.y < 0)
    //     // {
    //     //     viewportPos.y = 1;
    //     // }
    //     //
    //     // // return _camera.ViewportToWorldPoint(viewportPos);
    // }
}
