#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class BaseUIPanel
    {
        protected Stack<Color> Colors = new Stack<Color>();
        protected Stack<Color> BackgroundColors = new Stack<Color>();

        // Pushes current GUI.color value onto its stack base on input condition. If condition is false, the current color is pushed instead.
        public void PushColor(bool Condition, Color NewColor)
        {
            Color ColorToSet = NewColor;

            if (Condition == false)
            {
                ColorToSet = GUI.color;
            }

            Colors.Push(GUI.color);

            GUI.color = ColorToSet;
        }

        public void PopColor()
        {
            if (Colors.Count <= 0)
            {
                return;
            }

            Color PoppedColor = Colors.Pop();
            GUI.color = PoppedColor;
        }

        // Pushes current GUI.color value onto its stack base on input condition. If condition is false, the current color is pushed instead.
        public void PushBackgroundColor(bool Condition, Color NewBackgroundColor)
        {
            Color ColorToSet = NewBackgroundColor;

            if (Condition == false)
            {
                ColorToSet = GUI.backgroundColor;
            }

            BackgroundColors.Push(GUI.backgroundColor);

            GUI.backgroundColor = ColorToSet;
        }

        public void PopBackgroundColor()
        {
            if (BackgroundColors.Count <= 0)
            {
                return;
            }

            Color PoppedBackgroundColor = BackgroundColors.Pop();
            GUI.backgroundColor = PoppedBackgroundColor;
        }

        public void PushColorAndBackgroundColor(bool Condition, Color NewColor, Color NewBackgroundColor)
        {
            PushColor(Condition, NewColor);
            PushBackgroundColor(Condition, NewBackgroundColor);
        }
    }
}

#endif