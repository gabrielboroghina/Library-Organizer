using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LibraryOrganizer
{
    public partial class Delete : Form
    {
        const int MAX = 1000;

        DataTable table = new DataTable();
        DataGridViewButtonCell btn = new DataGridViewButtonCell();
        DataGridViewImageColumn del = new DataGridViewImageColumn();

        string[] searchedAuthor, searchedBook;
        int a = 0, c = 0, r;
        int[] res;

        public Delete()
        {
            InitializeComponent();
        }

        private bool text(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '-';
        }

        private bool found(int k)
        {
            int i;

            if (Main.el[k] == "zzz}") return false;

            for (i = 1; i <= a; i++)
                if (!Main.author.ContainsKey(Tuple.Create(k, searchedAuthor[i]))) return false;

            for (i = 1; i <= c; i++)
                if (!Main.book.ContainsKey(Tuple.Create(k, searchedBook[i]))) return false;

            return true;
        }

        private int min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        private void find()
        {
            int i;
            DataRow row;

            table.Clear();
            r = 0;

            for (i = 1; i <= Main.bookCount; i++)
                if (found(i)) res[++r] = i;

            for (i = 1; i <= min(r, 30); i++)    // Number of displayed items
            {
                row = table.NewRow();
                row["Author"] = Main.c_author[res[i]];
                row["Book title"] = Main.c_name[res[i]];
                table.Rows.Add(row);
            }

            if (r > 30)
            {
                row = table.NewRow();
                row["Book title"] = "Display all books";

                table.Rows.Add(row);
                btn.Dispose();
                btn = new DataGridViewButtonCell();
                dataGridView1[1, 30] = btn;
            }
        }

        private void Delete_Load(object sender, EventArgs e)
        {
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType,  
            // ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Author";
            table.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "Book title";
            table.Columns.Add(column);


            // Create new DataRow objects and add to DataTable.     
            for (int i = 1; i <= Main.bookCount; i++)
            {
                row = table.NewRow();
                row["Author"] = Main.c_author[i];
                row["Book title"] = Main.c_name[i];
                table.Rows.Add(row);
            }

            // Set DataSource property to the table.
            dataGridView1.DataSource = table;

            Image img = Image.FromFile("del.ico");
            del.Image = img;

            dataGridView1.Columns.Add(del);
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[2].Width = 40;

            searchedAuthor = new string[10];
            searchedBook = new string[10];
            res = new int[MAX];

            for (int i = 1; i <= Main.bookCount; i++) res[i] = i;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            StringBuilder word = new StringBuilder();
            string authorName;
            int i;

            a = 0;
            authorName = textBox1.Text;

            for (i = 0; i < authorName.Length;)
            {
                word.Clear();
                for (; i < authorName.Length && text(authorName[i]); i++)
                    word.Append(authorName[i]);

                searchedAuthor[++a] = word.ToString().ToUpper();
                for (; i < authorName.Length && !text(authorName[i]); i++) ;
            }
            find();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            StringBuilder word = new StringBuilder();
            string bookTitle;
            int i;

            c = 0;
            bookTitle = textBox2.Text;

            for (i = 0; i < bookTitle.Length;)
            {
                word.Clear();
                for (; i < bookTitle.Length && text(bookTitle[i]); i++)
                    word.Append(bookTitle[i]);

                searchedBook[++c] = word.ToString().ToUpper();
                for (; i < bookTitle.Length && !text(bookTitle[i]); i++) ;
            }
            find();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex == 30 && dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                table.Rows[30].Delete();
                DataRow row;
                for (int i = 31; i <= r; i++)
                {
                    row = table.NewRow();
                    row["Author"] = Main.c_author[res[i]];
                    row["Book title"] = Main.c_name[res[i]];
                    table.Rows.Add(row);
                }
            }
            else if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                int j = res[dataGridView1.CurrentCell.RowIndex + 1];
                DialogResult confirm = MessageBox.Show("Delete the book " + Main.el[j] + " ?",
                    "Confirm deletetion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    Main.el[j] = "zzz}";
                    MessageBox.Show("Book deleted!", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    find();
                }
            }
        }
    }
}
