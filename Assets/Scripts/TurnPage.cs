using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace MagicSim_Feature_Book
{
    public class TurnPage : MonoBehaviour, IMixedRealityPointerHandler
    {
        Vector3 lastPosition = Vector3.zero;
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, eventData.Handedness, out var pose);
            if (transform.parent.rotation.eulerAngles.z < -45 && transform.parent.rotation.eulerAngles.z < -125 || transform.parent.rotation.eulerAngles.y > 45 && transform.parent.rotation.eulerAngles.y < 125)
            {
                transform.Rotate(new Vector3(pose.Position.x * 2, 0, 0));
            }
            else
            {
                transform.Rotate(new Vector3(pose.Position.z * 2, 0, 0));
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {

        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {

        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {

        }
    }
}
