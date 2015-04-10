using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WheatstoneCode
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        //матрица алфавита шифрования
        public string[,] encriptionMatrixIn =
                                         {
                                         {"А", "Ч", "Б", "М", "Ц", "В"}, //первая строка матрицы
                                         {"Ь", "Г", "Н", "Ш", "Д", "О"}, //вторая строка матрицы
                                         {"Е", "Щ", ",", "Х", "У", "П"}, //третья строка матрицы
                                         {".", "З", "Ъ", "Р", "И", "Й"}, //четвертая строка матрицы
                                         {"С", "-", "К", "Э", "Т", "Л"}, //пятая строка матрицы
                                         {"Ю", "Я", " ", "Ы", "Ф", "Ж"}  //шестая строка матрицы
                                         };
        private string[,] encriptionMatrixOut = new string[6,6];

        private string alfavit = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ .-,";
        private string keyText;
        private string text; //исходный текст для шифрования

        private int i_first = 0, j_first = 0;  //координаты первого символа пары
        private int i_second = 0, j_second = 0;//координаты второго символа пары
        private string s1 = "", s2 = ""; //строки для хранения зашифрованного символа 
        private string encodetString; //зашифрованая строка
        private string decodetString; //расшифрованная строка

	
        #region Кодирование текста
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            keyText = Convert.ToString(textBox1.Text).ToUpper();
            int i, j;
            // Создание второго массива с помощью ключа.
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 6; j++)
                {
                    if (keyText.Length > 0 )
                    {
                        encriptionMatrixOut[i, j] = Convert.ToString(keyText[0]);
                        alfavit = alfavit.Replace(Convert.ToString(keyText[0]), "");
                        keyText = keyText.Replace(Convert.ToString(keyText[0]), "");
                    }
                    else
                    {
                        if (alfavit.Length > 0)
                        {
                            encriptionMatrixOut[i, j] = Convert.ToString(alfavit[0]);
                            alfavit = alfavit.Replace(Convert.ToString(alfavit[0]), "");
                        }
                    }
                }
            }
            int count = 0;
            foreach (string s in encriptionMatrixIn)
            {
                count++;
                if (count != 6)
                {
                    richTextBox1.Text += s + "\t";
                    //richTextBox1.Text += s;
                }
                if (count == 6)
                {
                    richTextBox1.Text += s + "\n";
                    //richTextBox1.Text += s + "\n";
                    count = 0;
                }
            }
            count = 0;
            foreach (string s in encriptionMatrixOut)
            {
                count++;
                if (count != 6)
                {
                    richTextBox2.Text += s + "\t";
                }
                if (count == 6)
                {
                    richTextBox2.Text += s + "\n";
                    count = 0;
                }
            }
        }



        #endregion
    }
}
