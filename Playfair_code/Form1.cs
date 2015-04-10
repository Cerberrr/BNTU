using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Playfair_code
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //матрица алфавита шифрования
       private string[,] encriptionMatrix =
                                         {
                                         {"А", "Ч", "Б", "М", "Ц", "В"}, //первая строка матрицы
                                         {"Ь", "Г", "Н", "Ш", "Д", "О"}, //вторая строка матрицы
                                         {"Е", "Щ", ",", "Х", "У", "П"}, //третья строка матрицы
                                         {".", "З", "Ъ", "Р", "И", "Й"}, //четвертая строка матрицы
                                         {"С", "-", "К", "Э", "Т", "Л"}, //пятая строка матрицы
                                         {"Ю", "Я", " ", "Ы", "Ф", "Ж"}  //шестая строка матрицы
                                         };
       private string text; //исходный текст для шифрования
      
       private int i_first = 0, j_first = 0;  //координаты первого символа пары
       private int i_second = 0, j_second = 0;//координаты второго символа пары
       private string  s1 = "", s2 = ""; //строки для хранения зашифрованного символа 
       private string encodetString; //зашифрованая строка
       private string decodetString; //расшифрованная строка
       private string login; //логин
       private string pass; //пароль

       #region Кодирование текста

       private void button1_Click(object sender, EventArgs e)
       {
            text = "";
            encodetString = "";
         
            text = Convert.ToString(richTextBox1.Text).ToUpper();
            int t = text.Length; //длина входного слова
            int i, j;
            ///проверяем, четное ли число символов в строке
            int temp = t % 2;
            if (temp != 0) //если нет
            {               //то добавляем в конец строки символ " " 
                text = text.PadRight((t + 1), ' ');
            }

            int len = text.Length / 2; /*длина нового массива -
                                                равная половине длины входного слова
                                                 т.к. в новом масиве каждый элемент будет
                                                   содержать 2 элемента из старого массива*/

            string[] str = new string[len]; //новый массив

            int l = -1; //служебная переменная

            for (i = 0; i < t; i += 2) //в старом массиве шаг равен 2
            {
                l++; //индексы для нового массива
                if (l < len)
                {
                    //Элемент_нового_массива[i] =  Элемент_старого_массива[i] +  Элемент_старого_массива[i+1]
                    str[l] = Convert.ToString(text[i]) + Convert.ToString(text[i + 1]);
                }

            }

            ///координаты очередного найденного символа из каждой пары

            foreach (string both in str)
            {
                for (i = 0; i < 6; i++)
                {
                    for (j = 0; j < 6; j++)
                    {
                        //координаты первого символа пары в исходной матрице
                        if (both[0] == Convert.ToChar(encriptionMatrix[i, j]))
                        {
                            i_first = i;
                            j_first = j;
                           
                        }

                        //координаты второго символа пары в исходной матрице
                        if (both[1] == Convert.ToChar(encriptionMatrix[i, j]))
                        {
                            i_second = i;
                            j_second = j;
                           
                        }
                    }
                }

                ///если пара символов находится в одной строке
                if (i_first == i_second)
                {
                    if (j_first == 5) /*если символ последний в строке,
                                       кодируем его первым символом из матрицы*/
                    {
                        s1 = Convert.ToString(encriptionMatrix[i_first, 0]);
                    }
                    //если символ не последний, кодируем его стоящим справа от него
                    else
                    {
                        s1 = Convert.ToString(encriptionMatrix[i_first, j_first + 1]);
                    }

                    if (j_second == 5) /*если символ последний в строке
                                       кодируем его первым символом из матрицы*/
                    {
                        s2 = Convert.ToString(encriptionMatrix[i_second, 0]);
                    }
                    //если символ не последний, кодируем его стоящим справа от него
                    else
                    {
                        s2 = Convert.ToString(encriptionMatrix[i_second, j_second + 1]);
                    }

                }

                ///если пара символов находится в одном столбце
                if (j_first == j_second)
                {
                    if (i_first == 5)
                    {
                        s1 = Convert.ToString(encriptionMatrix[0, j_first]);
                    }
                    else
                    {
                        s1 = Convert.ToString(encriptionMatrix[i_first + 1, j_first]);
                    }

                    if (i_second == 5)
                    {
                        s2 = Convert.ToString(encriptionMatrix[0, j_second]);
                    }

                    else
                    {
                        s2 = Convert.ToString(encriptionMatrix[i_second + 1, j_second]);
                    }
                }

                ///если пара символов находиться в разных столбцах и строках
                if (i_first != i_second && j_first != j_second)
                {

                    s1 = Convert.ToString(encriptionMatrix[i_first, j_second]);
                    s2 = Convert.ToString(encriptionMatrix[i_second, j_first]);
                }

                if (s1 == s2)
                {
                    encodetString = encodetString + s1 + "=" + s2;
                }
                else
                {

                    //записыавем результат кодирования
                    encodetString = encodetString + s1 + s2;
                }

                richTextBox2.Text = encodetString.ToLower();
            }
       }

       #endregion

        


       private void button3_Click(object sender, EventArgs e)
        {
                            
           
            int count = 0;
            login = Convert.ToString(textBox1.Text);
            pass = Convert.ToString(textBox2.Text);

            if (login == "root" && pass == "qwerty")
            {
                foreach (string s in encriptionMatrix)
                {
                    count++;
                    if (count != 6)
                    {
                        richTextBox3.Text += s + "\t";
                    }
                    if (count == 6)
                    { 
                    richTextBox3.Text += s + "\t\n";
                    count = 0;
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверный логин и пароль", "Ошибка инициализации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
            textBox2.Clear();

        }
        
    
        #region Раскодирование текста
        private void button2_Click_1(object sender, EventArgs e)
        {
            decodetString = "";
            string tempString = ""; //переменная для хранения строки подлежащей расшифровке
            int i, j;

            //удаляем символы "-" из зашифрованной строки
            for (i = 0; i < encodetString.Length; i++)
            {
                if (encodetString[i] != '=')
                {
                    tempString = tempString + encodetString[i].ToString();
                }
            }
            //MessageBox.Show(tempString);


            int len = tempString.Length / 2; /*длина нового массива -
                                                равная половине длины входного слова
                                                 т.к. в новом масиве каждый элемент будет
                                                   содержать 2 элемента из старого массива*/

            string[] str2 = new string[len]; //новый массив

            int l = -1; //служебная переменная

            for (i = 0; i < tempString.Length; i += 2) //в старом массиве шаг равен 2
            {
                l++; //индексы для нового массива
                if (l < len)
                {
                    //Элемент_нового_массива[i] =  Элемент_старого_массива[i] +  Элемент_старого_массива[i+1]
                    str2[l] = Convert.ToString(tempString[i]) + Convert.ToString(tempString[i + 1]);
                }

            }


            foreach (string both in str2)
            {
                for (i = 0; i < 6; i++)
                {
                    for (j = 0; j < 6; j++)
                    {
                        //координаты первого символа пары в исходной матрице
                        if (both[0] == Convert.ToChar(encriptionMatrix[i, j]))
                        {
                            i_first = i;
                            j_first = j;
                        }

                        //координаты второго символа пары в исходной матрице
                        if (both[1] == Convert.ToChar(encriptionMatrix[i, j]))
                        {
                            i_second = i;
                            j_second = j;
                        }
                    }
                }

                if (s1 == s2)
                {
                    if (i_first != 0 && i_second != 0)
                    {
                        s1 = Convert.ToString(encriptionMatrix[i_first - 1, j_first]);
                        s2 = Convert.ToString(encriptionMatrix[i_second - 1, j_second]);
                    }
                    else 
                    {
                        s1 = Convert.ToString(encriptionMatrix[5, j_first]);
                        s2 = Convert.ToString(encriptionMatrix[5, j_second]);
                    }

                }
                if (s1 != s2)
                {
                    if (i_first == i_second)
                    {
                        if (j_first == 0) /*если символ первый в строке,
                                       кодируем его последним символом из матрицы*/
                        {
                            s1 = Convert.ToString(encriptionMatrix[i_first, 5]);
                        }
                        //если символ не последний, кодируем его стоящим справа от него
                        else
                        {
                            s1 = Convert.ToString(encriptionMatrix[i_first, j_first - 1]);
                        }

                        if (j_second == 0) /*если символ последний в строке
                                       кодируем его первым символом из матрицы*/
                        {
                            s2 = Convert.ToString(encriptionMatrix[i_second, 5]);
                        }
                        //если символ не последний, кодируем его стоящим справа от него
                        else
                        {
                            s2 = Convert.ToString(encriptionMatrix[i_second, j_second - 1]);
                        }

                    }
                    ///если пара символов находится в одном столбце
                    if (j_first == j_second)
                    {
                        if (i_first == 0)
                        {
                            s1 = Convert.ToString(encriptionMatrix[5, j_first]);
                        }
                        else
                        {
                            s1 = Convert.ToString(encriptionMatrix[i_first - 1, j_first]);
                        }

                        if (i_second == 0)
                        {
                            s2 = Convert.ToString(encriptionMatrix[5, j_second]);
                        }

                        else
                        {
                            s2 = Convert.ToString(encriptionMatrix[i_second - 1, j_second]);
                        }
                    }

                    ///если пара символов находиться в разных столбцах и строках
                    if (i_first != i_second && j_first != j_second)
                    {

                        s1 = Convert.ToString(encriptionMatrix[i_first, j_second]);
                        s2 = Convert.ToString(encriptionMatrix[i_second, j_first]);

                    }
                }

               
                decodetString = decodetString + s1 + s2;
            }


            richTextBox4.Text = decodetString.ToLower();
        }
        #endregion
      

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

       
    }
}
