using StereoKit;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SKHands
{
    public class App
    {
        static List<(float time, HandJoint[] pose)> recordingHand = new List<(float time, HandJoint[] pose)>();

        public SKSettings Settings => new SKSettings
        {
            appName = "SKHands",
            assetsFolder = "Assets",
            displayPreference = DisplayMode.MixedReality
        };

        Model       leftHandModel;
        Model       rightHandModel;
        Material    floorMaterial;
        Material    jointMaterial;

        Pose        menuPose;
        Matrix4x4   floorTransform;

        struct JointInfo
        {
            public ModelNode node;
            public FingerId  finger;
            public JointId   joint;
            public JointInfo(FingerId fingerId, JointId jointId, ModelNode fingerNode)
            {
                finger = fingerId;
                joint  = jointId;
                node   = fingerNode;
            }
        }
        JointInfo[] jointInfoLeft;
        JointInfo[] jointInfoRight;

        float angleX = -90f;
        float angleY = 0f;
        float angleZ = 0f;
        float nodeScale = 1f;

        public void Init()
        {
            // Create assets used by the app
            leftHandModel = Model.FromFile("leftHand.glb");
            rightHandModel = Model.FromFile("rightHand.glb");
            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            jointMaterial = Material.Unlit.Copy();
            jointMaterial.DepthTest = DepthTest.Always;
            jointMaterial.QueueOffset = 1;

            floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));

            Input.HandVisible(Handed.Left, false);
            Input.HandVisible(Handed.Right, false);

            menuPose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(-1, 0, 1));

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
}

public void Step()
        {
            if (SK.System.displayType == Display.Opaque)
                Default.MeshCube.Draw(floorMaterial, floorTransform);

            UI.WindowBegin("Settings", ref menuPose);
            if (UI.Button("Dump Nodes"))
            {
                HandshotPose();
            }
            UI.Label("Global Node Angles");
            UI.HSlider("AngleXSlider", ref angleX, -90, 90, 0);
            UI.HSlider("AngleYSlider", ref angleY, -90, 90, 0);
            UI.HSlider("AngleZSlider", ref angleZ, -90, 90, 0);
            UI.Label("Node Scale");
            UI.HSlider("ScaleSlider", ref nodeScale, 0.1f, 1, 0);
            UI.WindowEnd();

            //// Uncomment for node and axis visualization
            //float scale = Hierarchy.ToLocalDirection(Vec3.UnitX).Magnitude;
            //for (int n = 0; n < leftHand.fingers.Length; n++)
            //{
            //    var node = Input.Hand(Handed.Left).fingers[n];
            //    Mesh.Cube.Draw(jointMaterial, node.Pose.ToMatrix(0.001f * scale));
            //    Lines.AddAxis(node.Pose, 0.01f);
            //    //Text.Add(node.Name, Matrix.S(scale) * node.ModelTransform, TextAlign.TopCenter, TextAlign.TopCenter);
            //}



            //for (int h = 0; h < (int)Handed.Max; h++)
            //{
            //Hand hand = Input.Hand((Handed)h);
            //foreach (JointInfo j in jointInfo)
            //{
            //    HandJoint joint = hand[j.finger, j.joint];
            //    j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * rot, nodeScale);
            //}

            //if (hand.tracked.IsActive())
            //    leftHandModel.Draw(Matrix.Identity);

            //}


            Quat rot = Quat.FromAngles(angleX, angleY, angleZ);
            Hand left = Input.Hand(Handed.Left);
            foreach (JointInfo j in jointInfoLeft)
            {
                HandJoint joint = left[j.finger, j.joint];
                j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * rot, nodeScale);
            }

            if (left.tracked.IsActive())
                leftHandModel.Draw(Matrix.Identity);

            Hand right = Input.Hand(Handed.Right);
            foreach (JointInfo j in jointInfoRight)
            {
                HandJoint joint = right[j.finger, j.joint];
                j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * rot, nodeScale);
            }

            if (right.tracked.IsActive())
                rightHandModel.Draw(Matrix.Identity);
        }

        // Unceremoniously ripped from https://github.com/maluoi/StereoKit/blob/master/Examples/StereoKitTest/DebugToolWindow.cs, just added some blender parsing.
        static void HandshotPose()
        {
            Hand h = Input.Hand(Handed.Left);
            HandJoint[] joints = new HandJoint[27];
            Array.Copy(h.fingers, 0, joints, 0, 25);
            joints[25] = new HandJoint(h.palm.position, h.palm.orientation, 0);
            joints[26] = new HandJoint(h.wrist.position, h.wrist.orientation, 0);
            recordingHand.Add((Time.Totalf, joints));

            string result = "----------------= Blender code starts here =----------------\n";
            for (int j = 0; j < joints.Length; j++)
            {
                result += $"bpy.ops.object.empty_add(type='PLAIN_AXES', radius=0.01, align='WORLD', location=({joints[j].position.x:0.000},{joints[j].position.y:0.000},{joints[j].position.z:0.000}), rotation=(0, 0, 0), scale=(1, 1, 1))";
                if (j < joints.Length - 1)
                    result += "\n";
            }
            result += "\n----------------= Blender code ends here =----------------";
            Log.Info(result);
        }
    }
}
