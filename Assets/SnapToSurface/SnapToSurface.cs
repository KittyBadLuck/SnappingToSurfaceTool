using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class SnapToSurface : MonoBehaviour
{
    public bool SnapRotation = false;
    public string tagToDetect = "Untagged";
    public float maxDistanceDetect = 10f;
    public float ClampOffset = 5;
    public Vector3 BasePosition;

    //adding this update stub makes the script toggle-able via the inspector
    void Update()
    {
        //stub to make the script toggle-able
    }
}

[CustomEditor(typeof(SnapToSurface))]
public class SnapToSurfaceEditor : Editor
{
    GUIContent tagsLabel = new("Tags To Detect", "Assign the same tag to the surface you want to detect");
    
    public override void OnInspectorGUI()
    {
        var snap = (SnapToSurface)target;
        
        snap.SnapRotation = EditorGUILayout.Toggle("Align Rotation To Surface", snap.SnapRotation);
        snap.ClampOffset = EditorGUILayout.FloatField("Offset From Surface", snap.ClampOffset);

        if (!couldFindCollidersInScene)
            EditorGUILayout.HelpBox($"No objects with the layer \"{snap.tagToDetect}\" found within a {snap.maxDistanceDetect} units.", MessageType.Warning);
        snap.tagToDetect = EditorGUILayout.TagField(tagsLabel, snap.tagToDetect);
        snap.maxDistanceDetect = EditorGUILayout.FloatField("Max Detection Distance", snap.maxDistanceDetect);
        //ensure the detection distance is never less than the clampOffset otherwise errors will occur
        snap.maxDistanceDetect = Mathf.Max(snap.maxDistanceDetect, snap.ClampOffset);
    }

    private bool couldFindCollidersInScene = true;
    private void OnSceneGUI()
    {
        var snap = (SnapToSurface)target;

        //Make so the script is toggle in inspector
        if (!snap.enabled)
        {
            return;
        }

        Collider targetCollider = FindClosestColliderWithTag(snap.maxDistanceDetect);
        couldFindCollidersInScene = targetCollider != null;
        if (!couldFindCollidersInScene)
        {
            return;
        }
        
        Vector3 targetClosestPoint = targetCollider.ClosestPoint(snap.BasePosition);
        
        //here we're making sure the basePosition is never within the collider, by using a raycast
        //the problem is that raycasts won't hit anything if they start within a collider
        //so we first have to cast the ray from outside the collider backtowards it
        Vector3 directionOutOfCollider = Vector3.Normalize(snap.transform.position - targetClosestPoint);
        Vector3 positionOutsideCollider = targetClosestPoint + directionOutOfCollider * targetCollider.bounds.extents.sqrMagnitude;
        Ray ray = new Ray(positionOutsideCollider, -directionOutOfCollider);
        if (targetCollider.Raycast(ray, out var hitInfo, Single.MaxValue))
        {
            snap.BasePosition = hitInfo.point;
        }
        else
        {
            snap.BasePosition = targetClosestPoint;
        }
        
        //technically a direction is always normalized so the normalization should happen on the below line, instead of after
        Vector3 directionFromSphereToSnapE = snap.transform.position - targetCollider.transform.position;
        Vector3 offsetClamped = directionFromSphereToSnapE.normalized * snap.ClampOffset;
        Vector3 offsetSlider;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, directionFromSphereToSnapE);
       
        snap.transform.position = targetClosestPoint + offsetClamped;
        if (snap.SnapRotation)
        {
            snap.transform.rotation = rotation;
        }

        EditorGUI.BeginChangeCheck();
        snap.BasePosition = Handles.PositionHandle(snap.BasePosition, Quaternion.identity);
        Handles.DrawLine(snap.BasePosition, snap.transform.position, 1);

        offsetSlider = Handles.Slider((snap.BasePosition + offsetClamped), directionFromSphereToSnapE, 3, Handles.ArrowHandleCap,0.001f);
        offsetSlider -= snap.BasePosition;
        snap.ClampOffset = offsetSlider.magnitude;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(snap, "Change Detected");
        }
        
        //Just find sphere if there is multiple
        Collider FindClosestColliderWithTag(float maxdistance)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag(snap.tagToDetect);
            Collider closest = null;
            Vector3 position = snap.transform.position;
            foreach (GameObject go in gos)
            {
                //we only care about gameObjects with colliders
                //so only return objects with colliders
                var collider = go.GetComponent<Collider>();
                if (collider == null)
                    continue;
                
                Vector3 diff = go.transform.position - position;
                //account for radius of sphere as well.
                float curDistance = diff.magnitude - go.transform.lossyScale.magnitude;
                if (curDistance < maxdistance)
                {
                    closest = collider;
                    maxdistance = curDistance;
                }
            }
            return closest;
            
        }

    }

}
#endif