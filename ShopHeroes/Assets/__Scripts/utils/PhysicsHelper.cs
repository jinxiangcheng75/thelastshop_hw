using UnityEngine;
using System.Collections;

public struct PhysicsHelper {
    public static GameObject CastRay (Camera cam, int layer) {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
#if UNITY_EDITOR
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);
#endif
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, 1 << layer)) {
            if(hit.collider != null) {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    public static GameObject CastRay2D (Camera cam, int layer) {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
#if UNITY_EDITOR
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);
#endif
        var hit2D = Physics2D.Raycast(ray.origin, ray.direction, 100, 1 << layer);
        if(hit2D && hit2D.collider != null) {
            return hit2D.collider.gameObject;
        }
        return null;
    }
}
