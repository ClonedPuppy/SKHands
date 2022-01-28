using StereoKit;

namespace SKHands
{
    public static class VRHands
    {
        static Model leftHandModel;
        static Model rightHandModel;

        struct JointInfo
        {
            public ModelNode node;
            public FingerId finger;
            public JointId joint;
            public JointInfo(FingerId fingerId, JointId jointId, ModelNode fingerNode)
            {
                finger = fingerId;
                joint = jointId;
                node = fingerNode;
            }
        }

        static JointInfo[] jointInfoLeft;
        static JointInfo[] jointInfoRight;
        static float nodeScale;
        static Quat defaultBoneRot;

        public static void init()
        {
            leftHandModel = Model.FromFile("leftHand.glb");
            rightHandModel = Model.FromFile("rightHand.glb");
            Input.HandVisible(Handed.Left, false);
            Input.HandVisible(Handed.Right, false);
            jointInfoLeft = new JointInfo[] {
                new JointInfo(FingerId.Thumb, JointId.KnuckleMajor, leftHandModel.FindNode("ThumbMeta")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMid,   leftHandModel.FindNode("ThumbProxi")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMinor, leftHandModel.FindNode("ThumbDist")),

                new JointInfo(FingerId.Index, JointId.Root,         leftHandModel.FindNode("IndexMeta")),
                new JointInfo(FingerId.Index, JointId.KnuckleMajor, leftHandModel.FindNode("IndexProxi")),
                new JointInfo(FingerId.Index, JointId.KnuckleMid,   leftHandModel.FindNode("IndexInter")),
                new JointInfo(FingerId.Index, JointId.KnuckleMinor, leftHandModel.FindNode("IndexDist")),

                new JointInfo(FingerId.Middle, JointId.Root,         leftHandModel.FindNode("MiddleMeta")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMajor, leftHandModel.FindNode("MiddleProxi")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMid,   leftHandModel.FindNode("MiddleInter")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMinor, leftHandModel.FindNode("MiddleDist")),

                new JointInfo(FingerId.Ring, JointId.Root,         leftHandModel.FindNode("RingMeta")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMajor, leftHandModel.FindNode("RingProxi")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMid,   leftHandModel.FindNode("RingInter")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMinor, leftHandModel.FindNode("RingDist")),

                new JointInfo(FingerId.Little, JointId.Root,         leftHandModel.FindNode("PinkyMeta")),
                new JointInfo(FingerId.Little, JointId.KnuckleMajor, leftHandModel.FindNode("PinkyProxi")),
                new JointInfo(FingerId.Little, JointId.KnuckleMid,   leftHandModel.FindNode("PinkyInter")),
                new JointInfo(FingerId.Little, JointId.KnuckleMinor, leftHandModel.FindNode("PinkyDist"))};

            jointInfoRight = new JointInfo[] {
                new JointInfo(FingerId.Thumb, JointId.KnuckleMajor, rightHandModel.FindNode("ThumbMeta")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMid, rightHandModel.FindNode("ThumbProxi")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMinor, rightHandModel.FindNode("ThumbDist")),

                new JointInfo(FingerId.Index, JointId.Root, rightHandModel.FindNode("IndexMeta")),
                new JointInfo(FingerId.Index, JointId.KnuckleMajor, rightHandModel.FindNode("IndexProxi")),
                new JointInfo(FingerId.Index, JointId.KnuckleMid, rightHandModel.FindNode("IndexInter")),
                new JointInfo(FingerId.Index, JointId.KnuckleMinor, rightHandModel.FindNode("IndexDist")),

                new JointInfo(FingerId.Middle, JointId.Root, rightHandModel.FindNode("MiddleMeta")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMajor, rightHandModel.FindNode("MiddleProxi")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMid, rightHandModel.FindNode("MiddleInter")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMinor, rightHandModel.FindNode("MiddleDist")),

                new JointInfo(FingerId.Ring, JointId.Root, rightHandModel.FindNode("RingMeta")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMajor, rightHandModel.FindNode("RingProxi")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMid, rightHandModel.FindNode("RingInter")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMinor, rightHandModel.FindNode("RingDist")),

                new JointInfo(FingerId.Little, JointId.Root, rightHandModel.FindNode("PinkyMeta")),
                new JointInfo(FingerId.Little, JointId.KnuckleMajor, rightHandModel.FindNode("PinkyProxi")),
                new JointInfo(FingerId.Little, JointId.KnuckleMid, rightHandModel.FindNode("PinkyInter")),
                new JointInfo(FingerId.Little, JointId.KnuckleMinor, rightHandModel.FindNode("PinkyDist"))};

            defaultBoneRot = Quat.FromAngles(-90f, 0, 0);
            nodeScale = 1;
        }

        public static void ShowHands()
        {
            Hand left = Input.Hand(Handed.Left);
            foreach (JointInfo j in jointInfoLeft)
            {
                HandJoint joint = left[j.finger, j.joint];
                j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);
            }

            if (left.tracked.IsActive())
                leftHandModel.Draw(Matrix.Identity);

            Hand right = Input.Hand(Handed.Right);
            foreach (JointInfo j in jointInfoRight)
            {
                HandJoint joint = right[j.finger, j.joint];
                j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);
            }

            if (right.tracked.IsActive())
                rightHandModel.Draw(Matrix.Identity);
        }
    }
}
