using Oculus.Interaction.Input;
using UnityEngine;

/// <summary>
/// Gives Surgical_table a solid body and keeps hands, controllers, and tools on top of it.
/// Tracked hands are posed after physics, so a collider alone cannot stop them.
/// </summary>
[DefaultExecutionOrder(32000)]
public class TableSurface : MonoBehaviour
{
    static readonly HandJointId[] FingerTips =
    {
        HandJointId.HandThumbTip,
        HandJointId.HandIndexTip,
        HandJointId.HandMiddleTip,
        HandJointId.HandRingTip,
        HandJointId.HandPinkyTip,
        HandJointId.HandWristRoot
    };

    BoxCollider solid;
    Collider[] hits = new Collider[48];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        MeshFilter[] filters = FindObjectsOfType<MeshFilter>();
        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i].gameObject.name != "Surgical_table")
                continue;
            if (filters[i].GetComponent<TableSurface>() == null)
                filters[i].gameObject.AddComponent<TableSurface>();
        }
    }

    void Awake()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null || filter.sharedMesh == null)
            return;

        Transform existing = transform.Find("TableCollider");
        if (existing != null)
        {
            solid = existing.GetComponent<BoxCollider>();
            return;
        }

        var colliderObject = new GameObject("TableCollider");
        colliderObject.transform.SetParent(transform, false);
        solid = colliderObject.AddComponent<BoxCollider>();
        solid.center = filter.sharedMesh.bounds.center;
        solid.size = filter.sharedMesh.bounds.size;
    }

    void LateUpdate()
    {
        if (solid == null)
            return;

        Physics.SyncTransforms();
        KeepBodiesOut();
        KeepHandsOut();
    }

    void KeepBodiesOut()
    {
        Bounds bounds = solid.bounds;
        int count = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, hits, solid.transform.rotation, ~0, QueryTriggerInteraction.Collide);
        float surface = bounds.max.y;

        for (int i = 0; i < count; i++)
        {
            Collider hit = hits[i];
            if (hit == null || hit == solid || hit.transform.IsChildOf(transform))
                continue;

            Bounds hitBounds = hit.bounds;
            if (!InsideFootprint(hitBounds.center))
                continue;
            if (hitBounds.min.y >= surface - 0.0005f)
                continue;
            if (hitBounds.center.y < bounds.min.y)
                continue;

            float lift = surface - hitBounds.min.y;
            Rigidbody body = hit.attachedRigidbody;
            if (body != null)
            {
                body.position += Vector3.up * lift;
                Vector3 velocity = body.velocity;
                if (velocity.y < 0f)
                    velocity.y = 0f;
                body.velocity = velocity;
            }
            else
            {
                hit.transform.position += Vector3.up * lift;
            }
        }
    }

    void KeepHandsOut()
    {
        OVRCameraRig rig = FindObjectOfType<OVRCameraRig>();
        if (rig == null)
            return;

        LiftAnchor(rig.leftHandAnchor, rig.leftControllerAnchor);
        LiftAnchor(rig.rightHandAnchor, rig.rightControllerAnchor);
    }

    void LiftAnchor(Transform handAnchor, Transform controllerAnchor)
    {
        if (handAnchor == null)
            return;

        float lift = 0f;
        Hand hand = handAnchor.GetComponentInChildren<Hand>();
        if (hand != null)
        {
            for (int i = 0; i < FingerTips.Length; i++)
            {
                if (!hand.GetJointPose(FingerTips[i], out Pose pose))
                    continue;
                lift = Mathf.Max(lift, PointLift(pose.position));
            }
        }

        if (controllerAnchor != null)
            lift = Mathf.Max(lift, PointLift(controllerAnchor.position));

        if (lift > 0f)
            handAnchor.position += Vector3.up * lift;
    }

    float PointLift(Vector3 worldPoint)
    {
        if (!InsideFootprint(worldPoint))
            return 0f;

        float surface = solid.bounds.max.y;
        float floor = solid.bounds.min.y;
        if (worldPoint.y >= surface || worldPoint.y < floor)
            return 0f;

        return surface - worldPoint.y;
    }

    bool InsideFootprint(Vector3 worldPoint)
    {
        Vector3 local = solid.transform.InverseTransformPoint(worldPoint);
        Vector3 half = solid.size * 0.5f;
        Vector3 delta = local - solid.center;
        return Mathf.Abs(delta.x) <= half.x && Mathf.Abs(delta.z) <= half.z;
    }
}
