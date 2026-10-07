using System;
using System.Drawing;
using System.Windows.Forms;

namespace Recorder.Extensions
{
    public static class FormExtensions
    {
        /// <summary>
        /// Centers the child form relative to the specified parent form's position and dimensions.
        /// </summary>
        public static void CenterToParentForm(this Form childForm, Form parentForm)
        {
            if (childForm == null || parentForm == null)
                return;

            childForm.StartPosition = FormStartPosition.Manual;

            int centerX = parentForm.Location.X + (parentForm.Width - childForm.Width) / 2;
            int centerY = parentForm.Location.Y + (parentForm.Height - childForm.Height) / 2;

            childForm.Location = new Point(centerX, centerY);
        }

        /// <summary>
        /// Positions the form next to the parent form, on the side with more free space,
        /// centered vertically and clamped to the screen's working area.
        /// </summary>
        public static void PositionRelativeTo(this Form form, Form parent)
        {
            if (parent == null) return;

            // Get the working area of the screen where the parent form is located
            Screen currentScreen = Screen.FromControl(parent);
            Rectangle workArea = currentScreen.WorkingArea;

            // Calculate free space to the left and right of the parent form
            int spaceLeft = parent.Left - workArea.Left;
            int spaceRight = workArea.Right - parent.Right;

            // Place on the left if there is more space on the left, otherwise place on the right
            int targetX = spaceLeft > spaceRight
                ? parent.Left - form.Width
                : parent.Right;

            // Center the child form vertically relative to the parent form
            int targetY = parent.Top + (parent.Height - form.Height) / 2;

            // Clamp coordinates to prevent the child form from going off-screen
            targetX = Math.Max(workArea.Left, Math.Min(targetX, workArea.Right - form.Width));
            targetY = Math.Max(workArea.Top, Math.Min(targetY, workArea.Bottom - form.Height));

            form.Location = new Point(targetX, targetY);
        }
    }
}