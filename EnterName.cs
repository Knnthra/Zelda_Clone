using System;
using System.Drawing;
using System.Windows.Forms;

namespace EksamenJuni2010
{
    /// <summary>
    /// A dialog box to be able to enter a name.
    /// </summary>
    public partial class EnterName : Form
    {
        /// <summary>
        /// Represents the name entered in the textbox.
        /// </summary>
        private string enteredName;

        /// <summary>
        /// Initializes a new instance of the EnterName class.
        /// </summary>
        public EnterName()
        {
            InitializeComponent();

            this.ActiveControl = this.tb_name;
        }

        /// <summary>
        /// Gets the entered name.
        /// </summary>
        public string EnteredName
        {
            get { return this.enteredName; }
        }

        /// <summary>
        /// If the entered name is valid, saves the entered name and closes the form.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (tb_name.Text.Length < 2)
            {
                lbl_errMsg.ForeColor = Color.Red;
                lbl_errMsg.Text = "Name too short.";
                this.Height = 122;
            }
            else if (tb_name.Text.Length > 10)
            {
                lbl_errMsg.ForeColor = Color.Red;
                lbl_errMsg.Text = "Name too long.";
                this.Height = 122;
            }
            else
            {
                this.enteredName = this.tb_name.Text;
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Sets the DialogResult to cancel, and closes the form.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}