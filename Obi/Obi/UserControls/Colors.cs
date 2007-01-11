using System;
using System.Drawing;

namespace Obi.UserControls
{
    public class Colors
    {
        public static Color AnnotationBlockUsed = Color.DeepSkyBlue;
        public static Color AnnotationBlockUnused = Color.LightGray;
        public static Color AudioBlockUsed = Color.Pink;
        public static Color AudioBlockUnused = Color.LightGray;
        public static Color AudioBlockEmpty = Color.MediumPurple;
        public static Color SectionStripUsed = Color.Gold;  // try: Color.Wheat;
        public static Color SectionStripUnused = Color.Gainsboro;  // lighter than light gray apparently

        public static Color SelectionColor = Color.Red;
        public static int SelectionWidth = 4;

        public static Color SectionUsed = Color.SteelBlue;
        public static Color SectionUnused = Color.Silver;
    }
}
