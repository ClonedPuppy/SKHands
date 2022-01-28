using StereoKit;
using System.Numerics;

namespace SKHands
{
    public class App
    {
        public SKSettings Settings => new SKSettings
        {
            appName = "SKHands",
            assetsFolder = "Assets",
            displayPreference = DisplayMode.MixedReality
        };

        Material    floorMaterial;
        Matrix4x4   floorTransform;

        public void Init()
        {
            //// Create assets used by the app
            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;
            floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));
            VRHands.init();
            Utils.init();
            
        }

        public void Step()
        {
            if (SK.System.displayType == Display.Opaque)
                Default.MeshCube.Draw(floorMaterial, floorTransform);
            VRHands.ShowHands();
            Utils.ShowAxis();
        }
    }
}
