using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AprilTag {

//
// Job struct that wraps AprilTag pose estimator
//
struct PoseEstimationJob : Unity.Jobs.IJobParallelFor
{
    // Input data struct that simply wraps pointers to tag detection data
    public struct Input
    {
        unsafe Interop.Detection* p;

        unsafe public Input(ref Interop.Detection r)
          => p = (Interop.Detection*)Interop.Util.AsPointer(ref r);

        unsafe public ref Interop.Detection Ref
          => ref Interop.Util.AsRef<Interop.Detection>(p);
    }

    // I/O
    [ReadOnly] NativeArray<Input> _input;
    [WriteOnly] NativeArray<TagPose> _output;

    // Camera parameters
    double _tagSize;
    double _fx, _fy;
    double2 _focalCenter;

    // Constructor
    public PoseEstimationJob
      (NativeArray<Input> input, NativeArray<TagPose> output,
       float fx, float fy, float cx, float cy, float tagSize)
    {
        _input = input;
        _output = output;
        _tagSize = tagSize;
        _fx = fx;
        _fy = fy;
        _focalCenter = math.double2(cx, cy);
    }

    // Job execution method
    public void Execute(int i)
    {
        var info = new Interop.DetectionInfo(ref _input[i].Ref, _tagSize,
           _fx, _fy, _focalCenter.x, _focalCenter.y);

        using var pose = new Interop.Pose(ref info);

        var pos = pose.t.AsFloat3() * math.float3(1, -1, 1);

        var rot = math.quaternion(pose.R.AsFloat3x3());
        rot = rot.value * math.float4(-1, 1, -1, 1);

        _output[i] = new TagPose(_input[i].Ref.ID, pos, rot);
    }
}

} // namespace AprilTag
