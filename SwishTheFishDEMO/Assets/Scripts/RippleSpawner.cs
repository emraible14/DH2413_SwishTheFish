using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem rippleParticlePrefab;
    private ParticleSystem psMain;

    [SerializeField] private Camera _camera;

    [Header("Particle attributes")] 
    [SerializeField] private float maxRippleSpeed = 1700;
    [SerializeField] private float maxRippleSize = 3;
    
    [SerializeField] private float spawnRate = 0.5f;
    [SerializeField] private float minDistanceBetween = 0.1f;

    private Vector3 lastRipplePos = Vector3.negativeInfinity;
    
    private Vector3 lastMousePos = Vector3.zero;
    private float mouseSpeed = 0f;
    

    private void Awake()
    {
        var ps = Instantiate(rippleParticlePrefab, transform.position, Quaternion.identity, transform);
        psMain = ps;
    }
    
    // rule of three: (rippleSpeed * maxRippleSize) / maxRippleSpeed
    // maxRippleSpeed - maxRippleSize
    //  rippleSpeed   -  rippleSize
    private ParticleSystem.EmitParams CalculateRippleSettings(Vector3 position, float distance, float speed)
    {
        var emitParams = new ParticleSystem.EmitParams
        {
            position = position - transform.position,
            // startSize = Mathf.Clamp((speed * maxRippleSize) / maxRippleSpeed, 0, 3),
            startSize = 1
            // startColor = new Color32(255, 255, 255, 128)
        };
        return emitParams;
    }

    // Update is called once per frame
    void Update()
    {
        var currentMousePos = Input.mousePosition;
        mouseSpeed = Mathf.Abs(Vector3.Distance(currentMousePos, lastMousePos)) / Time.deltaTime;
        lastMousePos = currentMousePos;
        Debug.Log(mouseSpeed);
        
        var ray = _camera.ViewportPointToRay(_camera.ScreenToViewportPoint(currentMousePos));
        if (!Physics.Raycast(ray, out var hit)) return;
        
        var distanceTravelled = Vector3.Distance(lastRipplePos, hit.point);
        if (distanceTravelled < minDistanceBetween) return;
        
        var emitParams = CalculateRippleSettings(hit.point, distanceTravelled, mouseSpeed);
        psMain.Emit(emitParams, 1);
        lastRipplePos = hit.point;
    }
}
