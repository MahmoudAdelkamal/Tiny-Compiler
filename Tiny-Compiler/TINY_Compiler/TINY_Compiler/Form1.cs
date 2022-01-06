using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txt_Errors.Text = "";
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
            //string Code=textBox1.Text.ToLower();
            string Code = textBox1.Text;
            TINY_Compiler.Start_Compiling(Code);
            PrintTokens();
            //PrintLexemes();
            treeView1.Nodes.Add(Parser.PrintParseTree(TINY_Compiler.treeroot));
            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < TINY_Compiler.Tiny_Scanner.Tokens.Count; i++)
            {
               dataGridView1.Rows.Add(TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).Lex, TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).TokenType);
            }
        }

        void PrintErrors()
        {
            for(int i=0; i<Errors.Error_List.Count; i++)
            {
                txt_Errors.Text += Errors.Error_List[i];
                txt_Errors.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            TINY_Compiler.TokenStream.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
