using StereoKit;

namespace SKHands
{
    static class MenuWindows
    {
        static Pose mSettingsPose;

        static public void init()
        {
            mSettingsPose = new Pose(Vec3.Zero, Quat.FromAngles(0, 180, 0));
        }

        static public void showMainSettings()
        {
            UI.WindowBegin("MainSettings", ref mSettingsPose);
            UI.PanelBegin();
            UI.Label("Area 1");
            if (UI.Button("Button 1"))
            {
                Utils.HandshotPose(Handed.Left);
            };
            UI.PanelEnd();
            UI.PanelBegin();
            UI.Label("Area 2");
            UI.Button("Button 2");
            UI.PanelEnd();
            UI.WindowEnd();
        }
    }
}
