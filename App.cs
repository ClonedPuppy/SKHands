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

        Model hand;
        Material floorMaterial;

        Material jointMaterial;

        Pose menuPose;
        Pose leftHandPose;
        Matrix4x4 floorTransform;

        Hand leftHand;

        ModelNode palm;

        // Thumb Nodes
        ModelNode thumbMeta;
        ModelNode thumbProxi;
        ModelNode thumbDist;
        ModelNode thumbTip;

        // Index Nodes
        ModelNode indexMeta;
        ModelNode indexProxi;
        ModelNode indexInter;
        ModelNode indexDist;
        ModelNode indexTip;

        // Middle Nodes
        ModelNode middleMeta;
        ModelNode middleProxi;
        ModelNode middleInter;
        ModelNode middleDist;
        ModelNode middleTip;

        // Ring Nodes
        ModelNode ringMeta;
        ModelNode ringProxi;
        ModelNode ringInter;
        ModelNode ringDist;
        ModelNode ringTip;

        // Pinky Nodes
        ModelNode pinkyMeta;
        ModelNode pinkyProxi;
        ModelNode pinkyInter;
        ModelNode pinkyDist;
        ModelNode pinkyTip;

        float angleX = -90f;
        float angleY = 0f;
        float angleZ = 0f;
        float nodeScale = 1f;

        public void Init()
        {
            // Create assets used by the app
            hand = Model.FromFile("hand.glb");
            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            jointMaterial = Material.Unlit.Copy();
            jointMaterial.DepthTest = DepthTest.Always;
            jointMaterial.QueueOffset = 1;

            floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));

            Input.HandVisible(Handed.Left, false);

            leftHand = Input.Hand(Handed.Left);

            leftHandPose = Input.Hand(Handed.Left).palm;

            menuPose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(-1, 0, 1));

            palm = hand.FindNode("Palm");

            // Thumb Nodes
            thumbMeta = hand.FindNode("ThumbMeta");
            thumbProxi = hand.FindNode("ThumbProxi");
            thumbDist = hand.FindNode("ThumbDist");
            thumbTip = hand.FindNode("ThumbTip");

            // Index Nodes
            indexMeta = hand.FindNode("IndexMeta");
            indexProxi = hand.FindNode("IndexProxi");
            indexInter = hand.FindNode("IndexInter");
            indexDist = hand.FindNode("IndexDist");
            indexTip = hand.FindNode("IndexTip");

            // Middle Nodes
            middleMeta = hand.FindNode("MiddleMeta");
            middleProxi = hand.FindNode("MiddleProxi");
            middleInter = hand.FindNode("MiddleInter");
            middleDist = hand.FindNode("MiddleDist");
            middleTip = hand.FindNode("MiddleTip");

            // Ring Nodes
            ringMeta = hand.FindNode("RingMeta");
            ringProxi = hand.FindNode("RingProxi");
            ringInter = hand.FindNode("RingInter");
            ringDist = hand.FindNode("RingDist");
            ringTip = hand.FindNode("RingTip");

            // Pinky Nodes
            pinkyMeta = hand.FindNode("PinkyMeta");
            pinkyProxi = hand.FindNode("PinkyProxi");
            pinkyInter = hand.FindNode("PinkyInter");
            pinkyDist = hand.FindNode("PinkyDist");
            pinkyTip = hand.FindNode("PinkyTip");
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

            Hierarchy.Push(leftHandPose.ToMatrix());

            //// Uncomment for node and axis visualization
            //float scale = Hierarchy.ToLocalDirection(Vec3.UnitX).Magnitude;
            //for (int n = 0; n < leftHand.fingers.Length; n++)
            //{
            //    var node = Input.Hand(Handed.Left).fingers[n];
            //    Mesh.Cube.Draw(jointMaterial, node.Pose.ToMatrix(0.01f * scale));
            //    Lines.AddAxis(node.Pose, 0.05f);
            //    //Text.Add(node.Name, Matrix.S(scale) * node.ModelTransform, TextAlign.TopCenter, TextAlign.TopCenter);
            //}

            palm.ModelTransform = Input.Hand(Handed.Left). palm.ToMatrix();

            // Thumb
            thumbMeta.ModelTransform = Matrix.TRS(leftHand.Get(0, 0).position, leftHand.Get(0, 0).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            thumbProxi.ModelTransform = Matrix.TRS(leftHand.Get(0, 1).position, leftHand.Get(0, 1).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            thumbDist.ModelTransform = Matrix.TRS(leftHand.Get(0, 2).position, leftHand.Get(0, 2).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            thumbTip.ModelTransform = Matrix.TRS(leftHand.Get(0, 3).position, leftHand.Get(0, 3).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);

            // Index
            indexMeta.ModelTransform = Matrix.TRS(leftHand.Get(1, 0).position, leftHand.Get(1, 0).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            indexProxi.ModelTransform = Matrix.TRS(leftHand.Get(1, 1).position, leftHand.Get(1, 1).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            indexInter.ModelTransform = Matrix.TRS(leftHand.Get(1, 2).position, leftHand.Get(1, 2).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            indexDist.ModelTransform = Matrix.TRS(leftHand.Get(1, 3).position, leftHand.Get(1, 3).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            indexTip.ModelTransform = Matrix.TRS(leftHand.Get(1, 4).position, leftHand.Get(1, 4).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);

            // Middle
            middleMeta.ModelTransform = Matrix.TRS(leftHand.Get(2, 0).position, leftHand.Get(2, 0).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            middleProxi.ModelTransform = Matrix.TRS(leftHand.Get(2, 1).position, leftHand.Get(2, 1).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            middleInter.ModelTransform = Matrix.TRS(leftHand.Get(2, 2).position, leftHand.Get(2, 2).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            middleDist.ModelTransform = Matrix.TRS(leftHand.Get(2, 3).position, leftHand.Get(2, 3).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            middleTip.ModelTransform = Matrix.TRS(leftHand.Get(2, 4).position, leftHand.Get(2, 4).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);

            // Ring
            ringMeta.ModelTransform = Matrix.TRS(leftHand.Get(3, 0).position, leftHand.Get(3, 0).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            ringProxi.ModelTransform = Matrix.TRS(leftHand.Get(3, 1).position, leftHand.Get(3, 1).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            ringInter.ModelTransform = Matrix.TRS(leftHand.Get(3, 2).position, leftHand.Get(3, 2).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            ringDist.ModelTransform = Matrix.TRS(leftHand.Get(3, 3).position, leftHand.Get(3, 3).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            ringTip.ModelTransform = Matrix.TRS(leftHand.Get(3, 4).position, leftHand.Get(3, 4).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);

            // Pinky
            pinkyMeta.ModelTransform = Matrix.TRS(leftHand.Get(4, 0).position, leftHand.Get(4, 0).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            pinkyProxi.ModelTransform = Matrix.TRS(leftHand.Get(4, 1).position, leftHand.Get(4, 1).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            pinkyInter.ModelTransform = Matrix.TRS(leftHand.Get(4, 2).position, leftHand.Get(4, 2).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            pinkyDist.ModelTransform = Matrix.TRS(leftHand.Get(4, 3).position, leftHand.Get(4, 3).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);
            pinkyTip.ModelTransform = Matrix.TRS(leftHand.Get(4, 4).position, leftHand.Get(4, 4).orientation * Quat.FromAngles(angleX, angleY, angleZ), nodeScale);

            hand.Draw(Matrix.Identity);

            Hierarchy.Pop();
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
