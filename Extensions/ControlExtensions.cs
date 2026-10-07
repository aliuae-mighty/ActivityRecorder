using System.Linq;
using System.Windows.Forms;

namespace Recorder.Extensions
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Recursively finds a control of the specified type by its name.
        /// </summary>
        public static T FindControl<T>(this Control parent, string controlName) where T : Control
        {
            if (parent == null || string.IsNullOrWhiteSpace(controlName))
                return null;

            return parent.Controls.Find(controlName, searchAllChildren: true)
                          .OfType<T>()
                          .FirstOrDefault();
        }
    }
}