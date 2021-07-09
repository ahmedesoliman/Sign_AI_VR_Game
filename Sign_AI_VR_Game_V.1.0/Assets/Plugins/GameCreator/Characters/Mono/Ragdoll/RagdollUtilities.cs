namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class RagdollUtilities
    {
        public static Bounds GetBounds(Transform origin, params Transform[] points)
        {
            Bounds bounds = new Bounds();
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 point = origin.InverseTransformPoint(points[i].position);
                bounds.Encapsulate(point);
            }

            return bounds;
        }

        public static void GetDirection(Vector3 point, out int direction, out float distance)
        {
            direction = RagdollUtilities.LargestComponent(point);
            distance = point[direction];
        }

        public static Vector3 GetDirectionAxis(Vector3 point)
        {
            int direction = 0;
            float distance = 0.0f;

            RagdollUtilities.GetDirection(point, out direction, out distance);
            Vector3 axis = Vector3.zero;

            if (distance > 0) axis[direction] =  1.0f;
            if (distance < 0) axis[direction] = -1.0f;

            return axis;
        }

        // COMPONENT METHODS: ---------------------------------------------------------------------

        public static int SmallestComponent(Vector3 point)
        {
            int direction = 0;
            if (Mathf.Abs(point[1]) < Mathf.Abs(point[0])) direction = 1;
            if (Mathf.Abs(point[2]) < Mathf.Abs(point[direction])) direction = 2;
            return direction;
        }

        public static int LargestComponent(Vector3 point)
        {
            int direction = 0;
            if (Mathf.Abs(point[1]) > Mathf.Abs(point[0])) direction = 1;
            if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction])) direction = 2;
            return direction;
        }

        public static Bounds ProportionalBounds(Bounds bounds)
        {
            Vector3 size = bounds.size;
            size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0f;

            bounds.size = size;
            return bounds;
        }

        public static Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, Vector3 unitY, bool below)
        {
            int axis = RagdollUtilities.LargestComponent(bounds.size);
            float dotMax = Vector3.Dot(unitY, relativeTo.TransformPoint(bounds.max));
            float dotMin = Vector3.Dot(unitY, relativeTo.TransformPoint(bounds.min));

            if ((dotMax > dotMin) == below)
            {
                Vector3 min = bounds.min;
                min[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                bounds.min = min;
            }
            else
            {
                Vector3 max = bounds.max;
                max[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                bounds.max = max;
            }

            return bounds;
        }

    }
}