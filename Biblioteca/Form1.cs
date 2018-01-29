using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LibraryOrganizer
{
    public partial class Main : Form
    {
        const int MAX = 1000;

        static Tuple<int, string> p = new Tuple<int, string>(0, "");
        public static string[] c_author, c_name;
        public static string[] el;

        public static int comp(int a, int b)
        {
            return (String.Compare(el[a], el[b], true));
        }

        DataTable table = new DataTable("table");
        public static Dictionary<Tuple<int, string>, bool> author = new Dictionary<Tuple<int, string>, bool>();
        public static Dictionary<Tuple<int, string>, bool> book = new Dictionary<Tuple<int, string>, bool>();
        DataGridViewButtonCell btn = new DataGridViewButtonCell();

        public static int bookCount, a = 0, c = 0, r;
        int[] res, o;
        string[] searchedAuthor, searchedBook;

        public Main()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.Show();
        }
        public static bool text(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '-';
        }

        private void LoadData()
        {
            int i;
            string s = "v";
            StringBuilder word = new StringBuilder();
            StreamReader fin = new StreamReader("data.txt");

            bookCount = 0; // total number of books
            c_author = new string[MAX];
            c_name = new string[MAX];
            el = new string[MAX];

            while (s != null)
            {
                s = fin.ReadLine();
                if (s == null) break;
                bookCount++;
                c_author[bookCount] = s;
                el[bookCount] += s;

                // author name -------------- build the dictionary
                for (i = 0; i < s.Length;)
                {
                    word.Clear();
                    for (; i < s.Length && text(s[i]); i++)
                    {
                        word.Append(s[i]);
                        author[Tuple.Create(bookCount, word.ToString().ToUpper())] = true;
                    }
                    for (; i < s.Length && !text(s[i]); i++) ;
                }

                // book title -------------- build the dictionary
                s = fin.ReadLine();
                c_name[bookCount] = s; el[bookCount] += " " + s;

                for (i = 0; i < s.Length;)
                {
                    word.Clear();
                    for (; i < s.Length && text(s[i]); i++)
                    {
                        word.Append(s[i]);
                        book[Tuple.Create(bookCount, word.ToString().ToUpper())] = true;
                    }
                    for (; i < s.Length && !text(s[i]); i++) ;
                }
            }

            fin.Close();

            searchedAuthor = new string[10];
            searchedBook = new string[10];
            res = new int[MAX];
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadData();
            toolStripStatusLabel1.Text = "Number of books: " + bookCount.ToString();

            // Declare DataColumn and DataRow variables.
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
            for (int i = 1; i <= bookCount; i++)
            {
                row = table.NewRow();
                row["Author"] = c_author[i];
                row["Book title"] = c_name[i];
                table.Rows.Add(row);
            }

            // Set to DataGrid.DataSource property to the table.
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].Width = 200;
        }

        public bool found(int k)
        {
            int i;

            if (el[k] == "zzz}") return false;

            for (i = 1; i <= a; i++)
                if (!author.ContainsKey(Tuple.Create(k, searchedAuthor[i]))) return false;

            for (i = 1; i <= c; i++)
                if (!book.ContainsKey(Tuple.Create(k, searchedBook[i]))) return false;

            return true;
        }

        public int min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public void find()
        {
            int i;
            DataRow row;
            table.Clear();

            r = 0;
            for (i = 1; i <= bookCount; i++)
                if (found(i)) res[++r] = i;

            for (i = 1; i <= min(r, 30); i++)    // Number of displayed items
            {
                row = table.NewRow();
                row["Author"] = c_author[res[i]];
                row["Book title"] = c_name[res[i]];
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                    row["Author"] = c_author[res[i]];
                    row["Book title"] = c_name[res[i]];
                    table.Rows.Add(row);
                }
            }
        }

        private void adaugareCarteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add addForm = new Add();
            addForm.Show();
        }

        private void sort()
        {
            o = new int[bookCount + 1];
            for (int i = 1; i <= bookCount; i++) o[i] = i;
            Array.Sort(o, comp);
        }

        public static void save()
        {
            int i;
            string s;
            StringBuilder word = new StringBuilder();

            el[bookCount] = c_author[bookCount] + " " + c_name[bookCount];
            s = c_author[bookCount];

            for (i = 0; i < s.Length;)
            {
                word.Clear();
                for (; i < s.Length && text(s[i]); i++)
                {
                    word.Append(s[i]);
                    author[Tuple.Create(bookCount, word.ToString().ToUpper())] = true;
                }
                for (; i < s.Length && !text(s[i]); i++) ;
            }

            s = c_name[bookCount];
            for (i = 0; i < s.Length;)
            {
                word.Clear();
                for (; i < s.Length && text(s[i]); i++)
                {
                    word.Append(s[i]);
                    book[Tuple.Create(bookCount, word.ToString().ToUpper())] = true;
                }
                for (; i < s.Length && !text(s[i]); i++) ;
            }
        }

        private void Form1_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sort();
            StreamWriter fout = new StreamWriter("data.txt");

            for (int i = 1; i <= bookCount; i++)
            {
                if (el[o[i]] == "zzz}") break;
                fout.WriteLine(c_author[o[i]]);
                fout.WriteLine(c_name[o[i]]);
            }

            fout.Close();
        }

        private void deleteBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete deleteForm = new Delete();
            deleteForm.Show();
        }
    }
}
