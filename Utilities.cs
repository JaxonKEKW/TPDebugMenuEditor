using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DarkUI.Collections;
using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;
using DarkUI.Forms;
using DarkUI.Renderers;
#pragma warning disable CS8603 // Possible null reference return.

namespace TPMenuEditor;
internal class Utilities
{
    public static string ConvertHexToString(string hexInput, System.Text.Encoding encoding)
    {
        var numberChars = hexInput.Length;
        var bytes = new byte[numberChars / 2];
        for (var i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
        }
        return encoding.GetString(bytes);
    }

    public static bool saveFile(byte[] saveData, bool shouldDark)
    {

        string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
        DialogResult saveResult = 0;

        if (shouldDark)
        {
            saveResult = DarkMessageBox.ShowWarning("Are you sure you would like to save? The file will be saved as '" + strWorkPath + "\\Menu1.dat' and will be overwritten with the content in this program.", "Save changes?", DarkDialogButton.YesNoCancel);

        }
        else
        {
            saveResult = MessageBox.Show("Are you sure you would like to save? The file will be saved as '" + strWorkPath + "\\Menu1.dat' and will be overwritten with the content in this program.", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

        }
        if (saveResult == DialogResult.Yes)
        {
            File.WriteAllBytes("Menu1.dat", saveData);
            MessageBox.Show("The menu file has been saved to the same directory as this program.", "Saved successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
        else if (saveResult == DialogResult.No)
        {
            return false;
        }
        else if (saveResult == DialogResult.Cancel)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    public static string ConvertStringToHex(String strInput, System.Text.Encoding encoding)
    {
        return BitConverter.ToString(encoding.GetBytes(strInput)).Replace("-", String.Empty);
    }

    public int FindBytes(byte[] src, byte[] find)
    {
        var index = -1;
        var matchIndex = 0;
        // handle the complete source array
        for (var i = 0; i < src.Length; i++)
        {
            if (src[i] == find[matchIndex])
            {
                if (matchIndex == (find.Length - 1))
                {
                    index = i - matchIndex;
                    break;
                }
                matchIndex++;
            }
            else if (src[i] == find[0])
            {
                matchIndex = 1;
            }
            else
            {
                matchIndex = 0;
            }

        }
        return index;
    }

    public byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
    {
        byte[] dst = null;
        var index = FindBytes(src, search);
        if (index >= 0)
        {
            dst = new byte[src.Length - search.Length + repl.Length];
            // before found array
            Buffer.BlockCopy(src, 0, dst, 0, index);
            // repl copy
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            // rest of src array
            Buffer.BlockCopy(
                src,
                index + search.Length,
                dst,
                index + repl.Length,
                src.Length - (index + search.Length));
        }
        return dst;
    }

    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }
}
