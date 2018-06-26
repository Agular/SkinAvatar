using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class SkeletonViewController : MonoBehaviour {

    public float scale = 5.0f;
    public float lineWidth = 0.05f;
    public float boxScale = 0.3f;

    private Dictionary<JointType, JointType> _BoneMap = new Dictionary<JointType, JointType>()
{
    { JointType.FootLeft, JointType.AnkleLeft },
    { JointType.AnkleLeft, JointType.KneeLeft },
    { JointType.KneeLeft, JointType.HipLeft },
    { JointType.HipLeft, JointType.SpineBase },

    { JointType.FootRight, JointType.AnkleRight },
    { JointType.AnkleRight, JointType.KneeRight },
    { JointType.KneeRight, JointType.HipRight },
    { JointType.HipRight, JointType.SpineBase },

    { JointType.HandTipLeft, JointType.HandLeft }, //Need this for HandSates
    { JointType.ThumbLeft, JointType.HandLeft },
    { JointType.HandLeft, JointType.WristLeft },
    { JointType.WristLeft, JointType.ElbowLeft },
    { JointType.ElbowLeft, JointType.ShoulderLeft },
    { JointType.ShoulderLeft, JointType.SpineShoulder },

    { JointType.HandTipRight, JointType.HandRight }, //Needthis for Hand State
    { JointType.ThumbRight, JointType.HandRight },
    { JointType.HandRight, JointType.WristRight },
    { JointType.WristRight, JointType.ElbowRight },
    { JointType.ElbowRight, JointType.ShoulderRight },
    { JointType.ShoulderRight, JointType.SpineShoulder },

    { JointType.SpineBase, JointType.SpineMid },
    { JointType.SpineMid, JointType.SpineShoulder },
    { JointType.SpineShoulder, JointType.Neck },
    { JointType.Neck, JointType.Head },
};

    void Awake () {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            jointObj.transform.localScale = new Vector3(boxScale, boxScale, boxScale);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = this.transform;
        }
        DisableRendering();
    }

    public void RefreshSkeletonData(Avatar.Body body)
    {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Avatar.Joint srcJoint = body.Joints[jt];
            Avatar.Joint tgtJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                tgtJoint = body.Joints[_BoneMap[jt]];
            }
            Transform jointObj = this.transform.Find(jt.ToString());
            jointObj.localPosition = ScalePosition(srcJoint.Position);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (tgtJoint != null)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, ScalePosition(tgtJoint.Position));
                lr.startColor = GetColorForState(srcJoint.TrackingState);
                lr.endColor = GetColorForState(tgtJoint.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private Vector3 ScalePosition(Vector3 position, bool mirrored = true)
    {
        if (mirrored)
        {
            return new Vector3(position.x * scale, position.y * scale, position.z * -scale);
        }
        else
        {
            return new Vector3(position.x * scale, position.y * scale, position.z * scale);
        }
    }

    private Color GetColorForState(TrackingState state)
    {
        switch (state)
        {
            case TrackingState.Tracked:
                return Color.green;

            case TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    public void EnableRendering()
    {
        SetRendering(true);
    }

    public void DisableRendering()
    {
        SetRendering(false);
    }

    private void SetRendering(bool value)
    {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Transform jointObj = this.transform.Find(jt.ToString());
            jointObj.GetComponent<LineRenderer>().enabled = value;
            jointObj.GetComponent<MeshRenderer>().enabled = value;
        }
    }

}
