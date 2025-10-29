using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    [Header("Pathing")]
    [SerializeField] private Pathing path;
    [SerializeField] private int startingPointIndex = 0;
    
    private Transform walkingTarget;
    
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator)
        {
            animator.speed = Mathf.Clamp(speed / 3, 1, 5);
        }
        path.SetStartingPoint(startingPointIndex);
        transform.position = path.Walk().position;
        walkingTarget = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!path) return;
        Debug.DrawLine(transform.position, walkingTarget.position, Color.red);
        if (Vector3.Distance(transform.position, walkingTarget.position) < 0.1f)
        {
            walkingTarget = path.Walk();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, walkingTarget.position, speed * Time.deltaTime);
        var targetRotation = Quaternion.LookRotation(walkingTarget.position - transform.position);
        var str = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }
}
