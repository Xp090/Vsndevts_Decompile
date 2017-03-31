using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
namespace Vsndevts_Decompile
{
  public  class Decompiler
    {
      public void Decompile(string filepath)
        {
            string[] CompiledFile = null;
            string DecompiledFile = null;

            if (filepath == null)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "vsndevts_c files (*.vsndevts_c)|*.vsndevts_c|All files (*.*)|*.*";
                fileDialog.RestoreDirectory = true;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    filepath = fileDialog.FileName;
                }
                    
                else
                {
                    System.Environment.Exit(1);
                }
            }

            try {CompiledFile = File.ReadAllLines(filepath);}
            catch (Exception ex)
            {
                 MessageBox.Show("Could not read the file : " + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 System.Environment.Exit(1);
            }
                   
            try
            {

                for (int i = 0; i < CompiledFile.Length; i++)
                {
                    if (CompiledFile[i].StartsWith("}"))
                    {
                        DecompiledFile += InsertTabs(4) + @"}" + "\r\n"
                                        + InsertTabs(3) + @"}" + "\r\n"
                                        + InsertTabs(2) + @"}" + "\r\n"
                                        + InsertTabs(1) + @"}" + "\r\n"
                                        + @"}" + "\r\n";
                    }
                    if (CompiledFile[i].Contains("kv3"))
                    {
                        string fieldName = CompiledFile[i];
                        string tempName = fieldName.Substring(0, fieldName.LastIndexOf("."));
                        int startIndex = tempName.LastIndexOf("\0");
                        fieldName = fieldName.Substring(startIndex);
                        fieldName = fieldName.Substring(0, fieldName.IndexOf("<!--"));
                        fieldName = Regex.Replace(fieldName, @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]", string.Empty);
                        fieldName = fieldName.Trim(new Char[] { ' ', '}' });
                        DecompiledFile += "\"" + fieldName + "\"" + "\r\n"
                                        + @"{" + "\r\n"
                                        + InsertTabs(1) + @"""operator_stacks""" + "\r\n"
                                        + InsertTabs(1) + @"{" + "\r\n"
                                        + InsertTabs(2) + @"""update_stack""" + "\r\n"
                                        + InsertTabs(2) + @"{" + "\r\n"
                                        + InsertTabs(3) + @"""reference_operator""" + "\r\n"
                                        + InsertTabs(3) + @"{" + "\r\n"
                                        + InsertTabs(4) + @"""operator""" + InsertTabs(1) + @"""sos_reference_stack""" + "\r\n";
                    }

                    else if (CompiledFile[i].Contains("=") && Encoding.UTF8.GetByteCount(CompiledFile[i]) <= CompiledFile[i].Length)
                    {
                        List<string> input_values = new List<string>();
                        input_values = CompiledFile[i].Split('=').ToList();
                        if (input_values[0].Trim() == "type")
                        {
                            DecompiledFile += InsertTabs(4) + @"""reference_stack""" + InsertTabs(1) + input_values[1] + "\r\n"
                                                            + InsertTabs(4) + @"""operator_variables""" + "\r\n"
                                                            + InsertTabs(4) + @"{" + "\r\n";
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(input_values[1]))
                            {
                                input_values.RemoveAt(1);
                                i += 2;
                                do
                                {
                                    input_values.Add(CompiledFile[i].Substring(0, CompiledFile[i].Length - 1));
                                    i++;
                                } while (!CompiledFile[i].Contains("]"));

                            }

                            DecompiledFile += ConvertValues(input_values);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not decompile the file : " + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(1);
            }

            try
            {
                filepath = System.IO.Path.ChangeExtension(filepath, "vsndevts");
                System.IO.StreamWriter file = new System.IO.StreamWriter(filepath);
                file.WriteLine(DecompiledFile);
                file.Close();
                MessageBox.Show("File has been decompiled", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Environment.Exit(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the decompiled file : " + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(1);
            }
        }


        string InsertTabs(int tabscount)
        {
            string Tabs = null;
            for (int i = 1; i <= tabscount; i++)
            {
                Tabs += "\t";
            }
            return Tabs;
        }


        string ConvertValues(List<string> inputValues)
        {
            string output;
            output = InsertTabs(5) + string.Format("\"{0}\"", inputValues[0].Trim()) + "\r\n"
                                   + InsertTabs(5) + "{" + "\r\n"
                                   + InsertTabs(6) + @"""value""";
            if (inputValues.Count > 2)
            {
                output += "\r\n"
                + InsertTabs(6) + "{" + "\r\n";
                for (int i = 1; i < inputValues.Count; i++)
                {
                    output += InsertTabs(7) + string.Format(@"""value{0}""", i - 1) + InsertTabs(1) + inputValues[i].Replace("\\\\", "\\") + "\r\n";
                }
                output += InsertTabs(6) + @"}" + "\r\n";
            }
            else
            {
                output += InsertTabs(1) + inputValues[1].Replace("\\\\", "\\") + "\r\n";
            }
            output += InsertTabs(5) + @"}" + "\r\n";
            return output;
        }

    }
}
