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

        public static Color AbstractBlockSelectionColor = Color.Red;
        public static int AbstractBlockSelectionWidth = 4;

        // remove those:
        public static Color StripUsedUnselected = Color.PaleGreen;
        public static Color StripUsedSelected = Color.Lime;
        public static Color StripUnusedUnselected = Color.LightGray;
        public static Color StripUnusedSelected = Color.Silver;
        public static Color SectionUsed = Color.SteelBlue;
        public static Color SectionUnused = Color.Silver;
    }
}
