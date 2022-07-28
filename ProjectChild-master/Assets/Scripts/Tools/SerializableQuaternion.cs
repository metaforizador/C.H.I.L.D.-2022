using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Since unity doesn't flag the Quaternion as serializable, we
/// need to create our own version. This one will automatically convert
/// between Quaternion and SerializableQuaternion.
/// 
/// This script was downloaded from unity forums.
/// </summary>
[System.Serializable]
public struct SerializableQuaternion {

    public float x, y, z, w;

    public SerializableQuaternion(float rX, float rY, float rZ, float rW) {
        x = rX;
        y = rY;
        z = rZ;
        w = rW;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>string representation</returns>
    public override string ToString() {
        return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }

    /// <summary>
    /// Automatic conversion from SerializableQuaternion to Quaternion.
    /// </summary>
    /// <param name="rValue">serialized version of the quaternion</param>
    /// <returns>generated quaternion</returns>
    public static implicit operator Quaternion(SerializableQuaternion rValue) {
        return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    /// <summary>
    /// Automatic conversion from Quaternion to SerializableQuaternion.
    /// </summary>
    /// <param name="rValue">quaternion</param>
    /// <returns>serialized version of the quaternion</returns>
    public static implicit operator SerializableQuaternion(Quaternion rValue) {
        return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }
}