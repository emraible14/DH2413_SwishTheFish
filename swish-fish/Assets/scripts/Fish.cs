using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Boid))]
public class Fish : MonoBehaviour 
{
    private Boid m_boid = null;
    private MeshRenderer meshRenderer;

	void Start () 
    {
        m_boid = GetComponent<Boid>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
	
	void Update () 
    {
        Vector3 velocity = m_boid.Velocity;
        var normalizedVel = velocity.normalized;
        meshRenderer.material.color = new Color(normalizedVel.x, normalizedVel.y, normalizedVel.z);
        
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 6);
        }

        transform.position = m_boid.Position;
        transform.position += new Vector3(0, 0, transform.parent.position.z); //inherit z-position
	}
}
