using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;

namespace Avatar
{
    public class Joint
    {
        public JointType JointType { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public TrackingState TrackingState { get; set; }

        public Joint() { }

        public Joint(JointType jointType, Vector3 position, Quaternion rotation, TrackingState trackingState)
        {
            JointType = jointType;
            Position = position;
            Rotation = rotation;
            TrackingState = trackingState;
        }

        public override string ToString()
        {
            return JointType.ToString();
        }
    }

    public class Body
    {
        public ulong TrackingId { set; get; }
        public Dictionary<JointType, Joint> Joints { set; get; }
        public bool IsTracked { set; get; }

        public Body()
        {

            Joints = new Dictionary<JointType, Joint>();
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
            {
                Joints[jt] = new Joint();
                Joints[jt].JointType = jt;
            }
        }

        public Body(Dictionary<JointType, Joint> joints)
        {
            Joints = joints;
        }

        public Body(Windows.Kinect.Body windowsBody)
        {
            Dictionary<JointType, Joint> joints = new Dictionary<JointType, Joint>();

            foreach (KeyValuePair<JointType, Windows.Kinect.Joint> jt in windowsBody.Joints)
            {
                Vector3 position = new Vector3(jt.Value.Position.X, jt.Value.Position.Y, jt.Value.Position.Z);
                Quaternion rotation = new Quaternion(
                    windowsBody.JointOrientations[jt.Key].Orientation.X,
                    windowsBody.JointOrientations[jt.Key].Orientation.Y,
                    windowsBody.JointOrientations[jt.Key].Orientation.Z,
                    windowsBody.JointOrientations[jt.Key].Orientation.W);
                joints[jt.Key] = new Avatar.Joint(jt.Key, position, rotation, jt.Value.TrackingState);
            }

            Joints = joints;
        }
    }
}
