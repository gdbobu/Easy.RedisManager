using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Easy.Controls
{
    public class ResClass
    {
        public static Bitmap GetResObj(string name)
        {
            if (name == null || name == "")
                return null;
            return (Bitmap)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Image GetPNG(string name)
        {
            if (name == null || name == "")
                return null;
            return (Image)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Icon GetResToIcon(string name)
        {
            if (name == null || name == "")
                return null;
            return (Icon)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Icon GetIcon(string name)
        {
            if (name == null || name == "")
                return null;
            return (Icon)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Image GetResToImage(string name)
        {
            if (name == null || name == "")
                return null;
            return (Bitmap)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static string[] GetAllFace(string name)
        {
            return null;
        }

        public static Bitmap GetHead(string name)
        {
            if (name == null || name == "")
                name = "big1";
            return (Bitmap)Easy.Controls.Properties.Resources.ResourceManager.GetObject(name);
        }

        public static string[] GetAllHead(string name)
        {
            return null;
        }
    }
}
