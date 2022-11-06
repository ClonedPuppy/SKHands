using System;
using System.Collections.Generic;
using StereoKit;

namespace SKHands
{
    public static class Utils
    {
        static List<(float time, HandJoint[] pose)> recordingHand = new List<(float time, HandJoint[] pose)>();
        static Material jointMaterial;

        public static void init()
        {
            jointMaterial = Material.Unlit.Copy();
            jointMaterial.DepthTest = DepthTest.Always;
            jointMaterial.QueueOffset = 1;
        }

        public static void ShowAxis()
        {
            float scale = Hierarchy.ToLocalDirection(Vec3.UnitX).Magnitude;
            for (int n = 0; n < Input.Hand(Handed.Left).fingers.Length; n++)
            {
                var node = Input.Hand(Handed.Left).fingers[n];
                Mesh.Cube.Draw(jointMaterial, node.Pose.ToMatrix(0.001f * scale));
                Lines.AddAxis(node.Pose, 0.01f);
            }
        }

        public static void HandshotPose(Handed hand)
        {
            Hand h = Input.Hand(hand);
            HandJoint[] joints = new HandJoint[27];
            Array.Copy(h.fingers, 0, joints, 0, 25);
            joints[25] = new HandJoint(h.palm.position, h.palm.orientation, 0);
            joints[26] = new HandJoint(h.wrist.position, h.wrist.orientation, 0);
            recordingHand.Add((Time.Totalf, joints));

            string result = ($"Tests.Hand(new HandJoint[]{{");
            for (int j = 0; j < joints.Length; j++)
            {
                result += $"new HandJoint(V.XYZ({joints[j].position.x:0.000}f,{joints[j].position.y:0.000}f,{joints[j].position.z:0.000}f), new Quat({joints[j].orientation.x:0.000}f,{joints[j].orientation.y:0.000}f,{joints[j].orientation.z:0.000}f,{joints[j].orientation.w:0.000}f), {joints[j].radius:0.000}f)";
                if (j < joints.Length - 1)
                    result += ",";
            }
            result += "});";
            Log.Info(result);
        }
        
        public static void DumpNodes()
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
