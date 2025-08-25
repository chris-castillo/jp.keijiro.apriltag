using UnityEngine;

namespace AprilTag {

//
// Tag pose structure for storing an estimated pose
//
public struct TagPose
{
    public int ID { get; }
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }
    public int Hamming { get; }
    public float DecisionMargin { get; }
    public float ReprojectionError { get; }

    public TagPose(
        int id,
        Vector3 position,
        Quaternion rotation,
        int hamming,
        float decisionMargin,
        float reprojectionError)
    {
        ID = id;
        Position = position;
        Rotation = rotation;
        Hamming = hamming;
        DecisionMargin = decisionMargin;
        ReprojectionError = reprojectionError;
    }
}

} // namespace AprilTag
