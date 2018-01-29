using System;
using System.Windows.Forms;

namespace LibraryOrganizer
{
    public partial class Add : Form
    {
        public string a_nou, c_nou;

        public Add()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a_nou = textBox1.Text;
            c_nou = textBox2.Text;

            Main.c_author[++Main.bookCount] = a_nou;
            Main.c_name[Main.bookCount] = c_nou;
            Main.save();

            MessageBox.Show("The book was registered", "Save",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}