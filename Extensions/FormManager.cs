using System;
using System.Windows.Forms;

namespace Recorder.Extensions
{
    public static class FormManager
    {
        /// <summary>
        /// Universal method to open a new form or activate an existing one.
        /// </summary>
        /// <typeparam name="TForm">The type of form to open, inheriting from Form.</typeparam>
        /// <param name="controlName">The name of the control passed to the factory.</param>
        /// <param name="formFactory">Delegate (lambda) to initialize the form if it hasn't been created yet.</param>
        public static void ShowOrActivateForm<TForm>(string controlName, Func<string, TForm> formFactory)
            where TForm : Form
        {
            if (formFactory == null) throw new ArgumentNullException(nameof(formFactory));

            string formName = typeof(TForm).Name;

            // Check if a form of this type is already open
            if (Application.OpenForms[formName] is Form existingForm)
            {
                if (existingForm.WindowState == FormWindowState.Minimized)
                {
                    existingForm.WindowState = FormWindowState.Normal;
                }

                existingForm.Activate();
                existingForm.Show();
            }
            else
            {
                // If not open, create via factory and show
                TForm newForm = formFactory(controlName);
                newForm.Show();
            }
        }
    }
}