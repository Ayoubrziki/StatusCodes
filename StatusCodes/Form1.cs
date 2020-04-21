using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatusCodes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ConnectionString = @"Data Source=10.2.243.138;Initial Catalog=B3G.Fawri.BankingV2;Integrated Security=false;Persist Security Info=False;MultipleActiveResultSets=true;User ID=sa;Password=@Dmin123";
            var ClassStrings = string.Empty;
            var FilePath = textBox1.Text;
            //var FilePath = @"E:\V2\FawriBank\B3G.FawriSuite.Bank.Engine.Core\Audit\Consts\StatusCodes.cs";
            // var FilePath = @"C:\StatusCodes\StatusCodes.cs";
            var VariableName = string.Empty;
            using (var cn = new SqlConnection(ConnectionString))
            {
                var query = "SELECT sc.Code,sl.DefaultValue FROM Core.StatusCode sc INNER JOIN Core.statusLabel sl ON sc.StatusLabelId=sl.Id";
                var cmd = new SqlCommand(query, cn);
                try
                {
                    cn.Open();
                    Console.WriteLine("Connected to server");
                    var reader = cmd.ExecuteReader();
                    Console.WriteLine("Command executed successfuly");
                    ClassStrings = "namespace B3G.FawriSuite.Bank.Engine.Core.Audit.Consts\n{\n\tpublic class StatusCodes\n\t{\n";
                    Console.WriteLine("Preparing Class...");
                    Console.WriteLine(ClassStrings);
                    while (reader.Read())
                    {
                        VariableName = reader[1].ToString().Replace(".", "_");
                        VariableName = VariableName.Replace("'", "");
                        VariableName = VariableName.Replace(",", "");
                        VariableName = VariableName.Replace("\"", "");
                        VariableName = VariableName.Replace("/", "");
                        VariableName = VariableName.Replace("(", "");
                        VariableName = VariableName.Replace(")", "");
                        VariableName = VariableName.Replace("-", "_");
                        VariableName = VariableName.Replace("!", "");
                        VariableName = VariableName.Replace(" ", "");
                        VariableName = VariableName.Replace(":", "");
                        VariableName = VariableName.Replace("’", "");
                        VariableName = VariableName.Replace("°", "");
                        VariableName = VariableName.Replace("É", "E");
                        VariableName = VariableName.Replace("é", "e");
                        VariableName = VariableName.Replace("è", "e");
                        VariableName = VariableName.Replace("à", "a");
                        VariableName = VariableName.Replace("À", "A");
                        var prop = string.Format("\t\tpublic static string {0} = \"{1}\";\n", VariableName, reader[0].ToString());
                        var existProperty = ClassStrings.Contains(VariableName);
                        if (existProperty)
                        {
                            var t = prop.IndexOf('p');
                            prop = prop.Insert(t, "//");
                        }
                        ClassStrings += prop;
                        Console.WriteLine(prop);
                    }
                    ClassStrings += "\t}\n}";
                    Console.WriteLine("\t}\n}");
                    Console.WriteLine("Getting Data Successfuly..");
                    cn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (!Directory.Exists(FilePath.Substring(0, FilePath.LastIndexOf(@"\"))))
            {
                Directory.CreateDirectory(FilePath.Substring(0, FilePath.LastIndexOf(@"\")));
                Console.WriteLine("Directory Created Successfuly..");
            }
            else
                Console.WriteLine("Using Existing Directory..");
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Console.WriteLine("Old File Deleted Successfuly..");
            }
            using (var fs = new FileStream(FilePath+ @"\StatusCodes.cs", FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            {
                Console.WriteLine("New File Created Successefuly..");
                Console.WriteLine("New File Opened Successefuly..");
                sw.Write(ClassStrings);
                Console.WriteLine("Data Writed In File Successfuly..");
                Console.WriteLine("File Closed.");
                Console.WriteLine("Done. Your File Exist Into : {0}.", FilePath);
            }
            Console.ReadLine();
            richTextBox1.Text = ClassStrings;
        }
    }
}
