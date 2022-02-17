using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace MagicSim_Feature_OpenDrawingWindow
{
    public class OpenDrawWindow : MonoBehaviour
    {
        bool isOpen = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Right, out var pose) && !isOpen)
            {
                if (pose.Rotation.y > -0.1 && pose.Rotation.y < 0.1)
                {
                    transform.position = pose.Position;

                }
                isOpen = true;
            }
        }
    }
}
