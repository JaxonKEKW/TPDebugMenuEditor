using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Diagnostics;
using static TPMenuEditor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPMenuEditor;
public partial class Form2 : Form
{
    public bool ran = false;
    public int currentCellLength = 0;
    public string currentCellValue = "";
    public string leftofCurrentCell = "";
    public byte[] wholeData;
    public int rowIndex;
    public int columnIndex;
    public int currentColumnIndex;
    public BinaryReader reader;
    public Form2()
    {
        InitializeComponent();
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string textBoxDefault)
        {
            Form prompt = new Form()
            {
                BackColor = SystemColors.Control,
                Width = 500,
                Height = 175,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, Text = textBoxDefault, };
            Label textLabel2 = new Label() { Left = 350, Top = 20, Text = "Bytes: " + Encoding.GetEncoding(932).GetByteCount(textBox.Text).ToString() };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            var maxBytes = 64;
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel2);
            confirmation.Click += (sender, e) => { prompt.Close(); };

            textLabel2.ForeColor = Color.Green;
            textBox.TextChanged += (sender, e) =>
            {
                textLabel2.Text = "Bytes: " + Encoding.GetEncoding(932).GetByteCount(textBox.Text).ToString();
                if (Encoding.GetEncoding(932).GetByteCount(textBox.Text) > maxBytes)
                {
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
                if (Encoding.GetEncoding(932).GetByteCount(textBox.Text) <= maxBytes)
                {
                    textLabel2.ForeColor = Color.Green;
                }
            };
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        public static string ShowDialogInt(string text, string caption, int updownDefault)
        {
            Form prompt = new Form()
            {
                BackColor = Color.FromArgb(26, 26, 26),
                Width = 500,
                Height = 175,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            NumericUpDown updown = new NumericUpDown() { Left = 50, Top = 50, Width = 400, Value = updownDefault, Maximum = 255, Hexadecimal = true };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            var maxBytes = 3;
            prompt.Controls.Add(updown);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(confirmation);
            confirmation.Click += (sender, e) => { prompt.Close(); };
            return prompt.ShowDialog() == DialogResult.OK ? updown.Value.ToString() : "";
        }
    }

    private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
    {
        if (ran == true)
        {
            if (dataGridView1.CurrentCell.ColumnIndex >= 3)
            {
                currentCellLength = Encoding.GetEncoding(932).GetByteCount(dataGridView1.CurrentCell.Value.ToString());
                currentCellValue = dataGridView1.CurrentCell.Value.ToString();
                currentColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
                if (currentColumnIndex == 3)
                {
                    leftofCurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[(dataGridView1.CurrentCell.ColumnIndex - 2)].Value.ToString();
                }
                else if (currentColumnIndex == 4)
                {
                    leftofCurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[(dataGridView1.CurrentCell.ColumnIndex - 3)].Value.ToString();
                }
                else if (currentColumnIndex == 5)
                {
                    leftofCurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[(dataGridView1.CurrentCell.ColumnIndex - 4)].Value.ToString();
                }

                var SuccessfullyChanged = false;
                while (SuccessfullyChanged == false)
                {
                    var editCellValue = Prompt.ShowDialogInt("Enter value:", "Edit Cell", Convert.ToInt32(currentCellValue, 16));
                    if (editCellValue == "")
                    {
                        saveToolStripButton.Enabled = false;
                        SuccessfullyChanged = true;
                        break;
                    }
                    else if (Encoding.GetEncoding(932).GetByteCount(editCellValue) > 3)
                    {
                        saveToolStripButton.Enabled = false;
                        MessageBox.Show("New value is larger than 64 bytes (MAX).", "Error: File size increased", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (Encoding.GetEncoding(932).GetByteCount(editCellValue) <= 3)
                    {
                        saveToolStripButton.Enabled = true;
                        var find1 = BitConverter.ToString(Encoding.GetEncoding(932).GetBytes(leftofCurrentCell)).Replace("-", "");
                        MessageBox.Show("find1: " + find1);
                        MessageBox.Show("leftofcurrentcell: " + leftofCurrentCell);
                        int intVal = int.Parse(editCellValue);

                        var wholeString = BitConverter.ToString(wholeData);
                        wholeString = wholeString.Replace("-", "");
                        var indexOfSearch = wholeString.IndexOf(find1);

                        var copy = wholeString.Substring(indexOfSearch, 152);
                        if (currentColumnIndex == 3)
                        {
                            copy = copy.Remove(0x92, 2);
                            if (intVal.ToString("X").Length < 2)
                            {
                                copy = copy.Insert(0x92, "0");
                                copy = copy.Insert(0x93, intVal.ToString("X"));
                            }
                            else
                            {
                                copy = copy.Insert(0x92, intVal.ToString("X"));
                            }
                        }
                        else if (currentColumnIndex == 4)
                        {
                            copy = copy.Remove(0x94, 2);
                            if (intVal.ToString("X").Length < 2)
                            {
                                copy = copy.Insert(0x94, "0");
                                copy = copy.Insert(0x95, intVal.ToString("X"));
                            }
                            else
                            {
                                copy = copy.Insert(0x94, intVal.ToString("X"));
                            }
                        }
                        else
                        {
                            copy = copy.Remove(0x96, 2);
                            if (intVal.ToString("X").Length < 2)
                            {
                                copy = copy.Insert(0x96, "0");
                                copy = copy.Insert(0x97, intVal.ToString("X"));
                            }
                            else
                            {
                                copy = copy.Insert(0x96, intVal.ToString("X"));
                            }
                        }
                        MessageBox.Show("copy: " + copy);
                        MessageBox.Show("getencoding(932) byte count of editcellvalue: " + Encoding.GetEncoding(932).GetByteCount(editCellValue).ToString());

                        wholeString = wholeString.Remove(indexOfSearch, 152);
                        wholeString = wholeString.Insert(indexOfSearch, copy);

                        wholeString = wholeString.Replace("-", "");
                        wholeData = StringToByteArray(wholeString);

                        dataGridView1.CurrentCell.Value = editCellValue;
                        SuccessfullyChanged = true;
                        currentCellLength = 0;
                        currentCellValue = "";
                        leftofCurrentCell = "";
                    }
                }
            }
            else
            {
                currentCellLength = Encoding.GetEncoding(932).GetByteCount(dataGridView1.CurrentCell.Value.ToString());
                currentCellValue = dataGridView1.CurrentCell.Value.ToString();
                currentColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
                var SuccessfullyChanged = false;
                while (SuccessfullyChanged == false)
                {
                    var editCellValue = Prompt.ShowDialog("Enter value:", "Edit Cell", currentCellValue);
                    if (editCellValue == "")
                    {
                        saveToolStripButton.Enabled = false;
                        SuccessfullyChanged = true;
                        break;
                    }
                    else if (Encoding.GetEncoding(932).GetByteCount(editCellValue) > 63)
                    {
                        saveToolStripButton.Enabled = false;
                        MessageBox.Show("New value is larger than 64 bytes (MAX).", "Error: File size increased", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (Encoding.GetEncoding(932).GetByteCount(editCellValue) <= 0x47)
                    {
                        saveToolStripButton.Enabled = true;
                        var find1 = BitConverter.ToString(Encoding.GetEncoding(932).GetBytes(currentCellValue)).Replace("-", "");
                        var replace1 = BitConverter.ToString(Encoding.GetEncoding(932).GetBytes(editCellValue)).Replace("-", "");

                        var wholeString = BitConverter.ToString(wholeData);
                        wholeString = wholeString.Replace("-", "");
                        var indexOfSearch = wholeString.IndexOf(find1);

                        var copy = wholeString.Substring(indexOfSearch, 0x80);
                        copy = copy.Replace(find1, replace1);
                        while (Encoding.GetEncoding(932).GetByteCount(copy) < 0x80)
                        {
                            copy += "00";
                        }

                        copy = copy.Substring(0, 0x80);

                        wholeString = wholeString.Remove(indexOfSearch, 0x80);
                        wholeString = wholeString.Insert(indexOfSearch, copy);

                        wholeString = wholeString.Replace("-", "");
                        wholeData = StringToByteArray(wholeString);
                        var i = 0;
                        if (currentColumnIndex == 0)
                        {
                            while (i < 256)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == dataGridView1.CurrentCell.Value.ToString())
                                {
                                    if (dataGridView1.CurrentCell.RowIndex != i)
                                    {
                                        dataGridView1.Rows[i].Cells[0].Value = editCellValue;
                                    }
                                }
                                i += 1;
                            }
                        }
                        dataGridView1.CurrentCell.Value = editCellValue;
                        SuccessfullyChanged = true;
                        currentCellLength = 0;
                        currentCellValue = "";
                        leftofCurrentCell = "";
                    }
                }
            }
        }
    }

    private void openToolStripButton_Click(object sender, EventArgs e)
    {
        var list = new List<KeyValuePair<string, int>>();
        list.Clear();
        if (ran == true)
        {
            DialogResult saveResult = MessageBox.Show("Would you like to save your changes before opening a new file?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (saveResult == DialogResult.Yes)
            {
                var hasSaved = saveFile(wholeData, shouldDark: true);
                if (hasSaved)
                {
                    saveToolStripButton.Enabled = false;
                }
            }
            if (saveResult == DialogResult.No)
            {

            }
            if (saveResult == DialogResult.Cancel)
            {
                return;
            }
        }
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.Filter = "DAT Files (.dat)|*.dat|All Files (*.*)|*.*";
        openFileDialog1.FilterIndex = 1;
        openFileDialog1.Title = ("Open Menu1.dat file");
        openFileDialog1.FileName = ("Menu1.dat");

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            saveToolStripButton.Enabled = false;
            dataGridView1.Enabled = true;
            wholeData = File.ReadAllBytes(openFileDialog1.FileName);
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            reader = new BinaryReader(new FileStream(openFileDialog1.FileName, FileMode.Open));

            //Main column offsets
            var mainColumnOffset = 0x48;
            var mainColumnCountOffset = 0x91;
            reader.BaseStream.Position = 0x50;
            var currentByteCount = 0x50;
            var counter = 1;

            //Read
            while (currentByteCount < 0x2760)
            {
                var hexText = BitConverter.ToString(reader.ReadBytes(0x48)).Replace("-", null); //reader.ReadBytes(12)
                reader.BaseStream.Position = mainColumnCountOffset;
                var hexCount = BitConverter.ToString(reader.ReadBytes(1)).Replace("-", null); //reader.ReadBytes(12)
                reader.BaseStream.Position = 0x50;

                for (var i = 2; i <= hexText.Length; i += 2)
                {
                    hexText = hexText.Insert(i, " ");
                    i++;
                }

                hexText = hexText.Substring(0, hexText.IndexOf(" 00 "));
                hexText = Regex.Replace(hexText, @"\s", "");
                var n = 0;
                while (n < Convert.ToInt32(hexCount, 16))
                {
                    list.Add(new KeyValuePair<string, int>(ConvertHexToString(hexText, Encoding.GetEncoding(932)), Convert.ToInt32(hexCount, 16)));
                    n += 1;
                }
                currentByteCount += mainColumnOffset;
                mainColumnCountOffset += mainColumnOffset;
                counter++;
                reader.BaseStream.Position = currentByteCount;
                dataGridView1.Height += 24;
                this.Height = dataGridView1.Height + 117;
            }

            var m = 0;
            while (m < list.Count)
            {
                dataGridView1.Rows.Add(list[m].Key);
                m += 1;
            }
            //Sec column offsets
            var secColumnOffset = 0x4C;
            reader.BaseStream.Position = 0x27B4;
            currentByteCount = 0x27B4;
            counter = 1;
            var counter2 = 0;

            //Read
            while (currentByteCount < 0x27518)
            {
                var hexText = BitConverter.ToString(reader.ReadBytes(0x4C)).Replace("-", null); //reader.ReadBytes(12)

                for (var i = 2; i <= hexText.Length; i += 2)
                {
                    hexText = hexText.Insert(i, " ");
                    i++;
                }
                hexText = Regex.Replace(hexText, @"\s", "");
                dataGridView1.Rows[counter2].SetValues(dataGridView1.Rows[counter2].Cells[0].Value, ConvertHexToString(hexText.Substring(0, 0x80), Encoding.GetEncoding(932)));
                dataGridView1.Rows[counter2].Cells[2].Value = ConvertHexToString(hexText.Substring(0x82, 16), Encoding.GetEncoding(932));
                dataGridView1.Rows[counter2].Cells[3].Value = Convert.ToInt32(hexText.Substring(0x92, 2), 16);
                dataGridView1.Rows[counter2].Cells[4].Value = Convert.ToInt32(hexText.Substring(0x94, 2), 16);
                dataGridView1.Rows[counter2].Cells[5].Value = Convert.ToInt32(hexText.Substring(0x96, 2), 16);
                currentByteCount += secColumnOffset;
                counter++;
                counter2++;
                reader.BaseStream.Position = currentByteCount;
                dataGridView1.Height += 24;
                this.Height = dataGridView1.Height + 87;
            }




        }
        ran = true;
    }

    private void saveToolStripButton_Click(object sender, EventArgs e)
    {
        var hasSaved = saveFile(wholeData, shouldDark: true);

        if (hasSaved)
        {
            saveToolStripButton.Enabled = false;
        }
    }

    private void Form2_FormClosing(object sender, FormClosingEventArgs e)
    {
        DialogResult saveResult = 0;
        if (ran == true)
        {
            saveResult = MessageBox.Show("Do you wish to save your changes before exiting?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (saveResult == DialogResult.Yes)
            {
                saveFile(wholeData, shouldDark: true);
            }
            else if (saveResult == DialogResult.No)
            {

            }
            else if (saveResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {

    }

    private void switchtoDarkMode_Click(object sender, EventArgs e)
    {
        DialogResult darkmodeResult = MessageBox.Show("This will RESTART the program. You will lose unsaved progress. Are you sure you want to switch to Light Mode?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (darkmodeResult == DialogResult.Yes)
        {
            try
            {
                reader.Close();
            }
            catch
            {

            }
            ran = false;
            this.Hide();
            var form1 = new Form1();
            form1.Closed += (s, args) => this.Close();
            form1.Show();
        }
        else
        {

        }
    }
}