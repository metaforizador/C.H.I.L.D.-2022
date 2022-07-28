using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Since unity doesn't flag the Vector3 as serializable, we
/// need to create our own version. This one will automatically convert
/// between Vector3 and SerializableVector3.
/// 
/// This script was downloaded from unity forums.
/// </summary>
[System.Serializable]
public struct SerializableVector3 {

    public float x, y, z;

    public SerializableVector3(float rX, float rY, float rZ) {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>string represantion</returns>
    public override string ToString() {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3.
    /// </summary>
    /// <param name="rValue">serialized version of Vector3</param>
    /// <returns>generated Vector3</returns>
    public static implicit operator Vector3(SerializableVector3 rValue) {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3.
    /// </summary>
    /// <param name="rValue">Vector3 to convert</param>
    /// <returns>serialized version of the Vector3</returns>
    public static implicit operator SerializableVector3(Vector3 rValue) {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}