using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static Vector3 GetWorldPositionOnPlane(Camera camera, Vector3 screenPosition, float depth = 15)
    {
        var ray = camera.ViewportPointToRay(screenPosition);
        var xy = new Plane(Vector3.up, new Vector3(0, depth, 0));
        xy.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    public static Vector3 ReverseZIndex(Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z * -1);
    }
}
