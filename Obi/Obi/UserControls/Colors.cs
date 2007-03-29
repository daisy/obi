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
        public static Color SectionStripUsedBack = Color.Gold;  // try: Color.Wheat;
        public static Color SectionStripUnusedBack = Color.Gainsboro;  // lighter than light gray apparently
        public static Color SectionStripUsedFore = Color.Black;
        public static Color SectionStripUnusedFore = Color.Black;

        public static Color SelectionColor = Color.Red;
        // public static int SelectionWidth = 4;

        public static Color SectionUsed = Color.SteelBlue;
        public static Color SectionUnused = Color.Silver;

        // Avn: following part added to support high contrast, may be removed if not suitable
        public static Color ObiBackGround = Color.White ;


        public static void SetHighContrastColors ( bool IsSystemInHighContrast )
        {
            if (IsSystemInHighContrast)
            {
                AnnotationBlockUsed = Color.DeepSkyBlue;
                AnnotationBlockUnused = Color.LightGray;
                AudioBlockUsed = Color.Pink;
                AudioBlockUnused = Color.LightGray;
                AudioBlockEmpty = Color.MediumPurple;
                SectionStripUsedBack = Color.Gold;  // try: Color.Wheat;
                SectionStripUnusedBack = Color.Gainsboro;  // lighter than light gray apparently
                SectionStripUsedFore = Color.Black;
                SectionStripUnusedFore = Color.Black;

                SelectionColor = SystemColors.WindowText;


                SectionUsed = Color.SteelBlue;
                SectionUnused = Color.Silver;

                ObiBackGround = SystemColors.Window;
            }
            else
            {
                AnnotationBlockUsed = Color.DeepSkyBlue;
                AnnotationBlockUnused = Color.LightGray;
                AudioBlockUsed = Color.Pink;
                AudioBlockUnused = Color.LightGray;
                AudioBlockEmpty = Color.MediumPurple;
                SectionStripUsedBack = SystemColors.WindowFrame;
                SectionStripUnusedBack = Color.Gainsboro;  // lighter than light gray apparently
                SectionStripUsedFore = Color.Black;
                SectionStripUnusedFore = Color.Black;

                SelectionColor = Color.Red;


                SectionUsed = Color.SteelBlue;
                SectionUnused = Color.Silver;

                ObiBackGround = Color.White;
            }
        }


    }
}
