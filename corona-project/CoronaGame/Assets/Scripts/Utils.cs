using UnityEngine;

public static class Utils
{
    public static Vector3 Map(Vector3 val, float old_min, float old_max, float new_min, float new_max)
    {
        return new Vector3(Map(val.x, old_min, old_max, new_min, new_max),
            Map(val.y, old_min, old_max, new_min, new_max), Map(val.z, old_min, old_max, new_min, new_max));
    }

    public static float Map(float val, float old_min, float old_max, float new_min, float new_max)
    {
        return (val) / (old_max - old_min) * (new_max - new_min);
    }

    public static Vector3 Clamp(Vector3 val, float min, float max)
    {
        return new Vector3(Mathf.Clamp(val.x, min, max), Mathf.Clamp(val.y, min, max), Mathf.Clamp(val.z, min, max));
    }

    public static Vector3 Lerp(Vector3 val, float a, float b)
    {
        return new Vector3(Mathf.Lerp(a, b, val.x),
            Mathf.Lerp(a, b, val.y), Mathf.Lerp(a, b, val.z));
    }
}