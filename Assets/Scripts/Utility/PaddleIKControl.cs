using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(Animator))]
    public class PaddleIKControl : MonoBehaviour
    {
        protected Animator animator;

        public bool ikActive;
        public Transform paddle;
        public Transform rightHandObj;
        public Transform leftHandObj;
        public Transform lookObj;
        [Range(0f,1f)]
        public float weight = 1f;

        public float yMin = 0f;
        public float yMax = 115f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            //if the IK is active, set the position and rotation directly to the goal.
            if (ikActive)
            {
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                var y = paddle.localEulerAngles.y;
                if (y < yMin || y > yMax) return;

                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}