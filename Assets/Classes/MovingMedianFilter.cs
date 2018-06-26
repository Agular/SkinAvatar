using System.Collections.Generic;
using Windows.Kinect;

/*
    This filter is practically pointless. By using a medium we are using a precise value, but just X updates later.
    So in terms of smoothing and rounding, median does nothing.
    Just tried this for testing.]
*/


public class MovingMedianFilter : Filter
{
    public MovingMedianFilter(int maxSamples) : base(maxSamples)
    {
    }

    public override Avatar.Body GetFilteredData(Avatar.Body body)
    {
        AddToSamples(body);
        if (MaxSamples <= 1 || BodySamples.Count != MaxSamples)
        {
            return BodySamples[0];
        }

        return GetMovingMedianData();
    }

    private Avatar.Body GetMovingMedianData()
    {
        Dictionary<JointType, Avatar.Joint> medianJoints = new Dictionary<JointType, Avatar.Joint>();
        for (JointType jt = 0; jt <= JointType.ThumbRight; jt++)
        {
            medianJoints[jt] = new Avatar.Joint();
            medianJoints[jt].JointType = BodySamples[MaxSamples / 2].Joints[jt].JointType;
            medianJoints[jt].Position = BodySamples[MaxSamples / 2].Joints[jt].Position;
            medianJoints[jt].Rotation = BodySamples[MaxSamples / 2].Joints[jt].Rotation;
            medianJoints[jt].TrackingState = BodySamples[MaxSamples / 2].Joints[jt].TrackingState;
        }
        return new Avatar.Body(medianJoints);
    }
}
