using UnityEngine;
using Windows.Kinect;
using Microsoft.Kinect.Face;
using System.Collections.Generic;

public class BodyManager : MonoBehaviour
{

    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;

    private Body[] _Data = null;
    private Avatar.Body[] avatarBodies = null;

    private int bodyCount;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;

    public Body[] GetData()
    {
        return _Data;
    }

    public Avatar.Body[] GetAvatarBodies()
    {
        return avatarBodies;
    }

    void Awake()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }

        bodyCount = _Sensor.BodyFrameSource.BodyCount;
        FaceFrameFeatures faceFrameFeatures = FaceFrameFeatures.RotationOrientation;

        faceFrameSources = new FaceFrameSource[bodyCount];
        faceFrameReaders = new FaceFrameReader[bodyCount];

        avatarBodies = new Avatar.Body[bodyCount];
        for (int i = 0; i < bodyCount; i++)
        {
            faceFrameSources[i] = FaceFrameSource.Create(_Sensor, 0, faceFrameFeatures);
            faceFrameReaders[i] = faceFrameSources[i].OpenReader();
        }

        for (int i = 0; i < bodyCount; i++)
        {
            avatarBodies[i] = new Avatar.Body();
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
            {
                avatarBodies[i].Joints[jt] = new Avatar.Joint();
                avatarBodies[i].Joints[jt].JointType = jt;
            }
        }

    }

    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }
                frame.GetAndRefreshBodyData(_Data);
                UpdateAvatarBodies();
                frame.Dispose();
                frame = null;
            }


        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        for (int i = 0; i < bodyCount; i++)
        {
            if (faceFrameReaders[i] != null)
            {
                faceFrameReaders[i].Dispose();
                faceFrameReaders[i] = null;
            }
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }

    private void UpdateAvatarBodies()
    {
        for (int i = 0; i < bodyCount; i++)
        {
            avatarBodies[i].IsTracked = _Data[i].IsTracked;
            avatarBodies[i].TrackingId = _Data[i].TrackingId;
            foreach (KeyValuePair<JointType, Windows.Kinect.Joint> jt in _Data[i].Joints)
            {
                Vector3 position = new Vector3(jt.Value.Position.X, jt.Value.Position.Y, jt.Value.Position.Z);
                Quaternion rotation = new Quaternion(
                    _Data[i].JointOrientations[jt.Key].Orientation.X,
                    _Data[i].JointOrientations[jt.Key].Orientation.Y,
                    _Data[i].JointOrientations[jt.Key].Orientation.Z,
                    _Data[i].JointOrientations[jt.Key].Orientation.W);
                avatarBodies[i].Joints[jt.Key].Position = position;
                avatarBodies[i].Joints[jt.Key].Rotation = rotation;
                avatarBodies[i].Joints[jt.Key].TrackingState = jt.Value.TrackingState;
            }
        }
        UpdateAvatarBodiesWithHeadRotations();
    }

    private void UpdateAvatarBodiesWithHeadRotations()
    {
        for (int i = 0; i < bodyCount; i++)
        {
            if (faceFrameSources[i].IsTrackingIdValid)
            {
                using (FaceFrame faceFrame = faceFrameReaders[i].AcquireLatestFrame())
                {
                    if (faceFrame != null)
                    {
                        if (faceFrame.TrackingId == 0)
                        {
                            continue;
                        }
                        var result = faceFrame.FaceFrameResult;
                        Quaternion q = GetQuaternionFromVector4(result.FaceRotationQuaternion);
                        if (q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0  ||
                            float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w) )
                        {
                            avatarBodies[i].Joints[JointType.Head].Rotation = Quaternion.identity;
                        }
                        else
                        {
                            avatarBodies[i].Joints[JointType.Head].Rotation = q;
                        }
                    }
                }
            }
            else
            {
                if (_Data[i].IsTracked)
                {
                    faceFrameSources[i].TrackingId = _Data[i].TrackingId;
                }
            }
        }
    }


    private Quaternion GetQuaternionFromVector4(Windows.Kinect.Vector4 jointOrientation)
    {
        float ox = jointOrientation.X;
        float oy = jointOrientation.Y;
        float oz = jointOrientation.Z;
        float ow = jointOrientation.W;
        return new Quaternion(ox, oy, oz, ow);
    }
}
