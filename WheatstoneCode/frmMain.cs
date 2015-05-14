using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net; 
using System.Net.Sockets;

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
            richTextBox3.Text = "";
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
            text = "";
            encodetString = "";
            text = Convert.ToString(richTextBox4.Text).ToUpper();
            int t = text.Length; //длина входной строки
            // проверяем, четное ли число символов в строке
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
            // координаты очередного найденного символа из каждой пары

            foreach (string both in str)
            {
                for (i = 0; i < 6; i++)
                {
                    for (j = 0; j < 6; j++)
                    {
                        //координаты первого символа пары в исходной матрице 1
                        if (both[0] == Convert.ToChar(encriptionMatrixIn[i, j]))
                        {
                            i_first = i;
                            j_first = j;

                        }

                        //координаты второго символа пары в исходной матрице 2
                        if (both[1] == Convert.ToChar(encriptionMatrixOut[i, j]))
                        {
                            i_second = i;
                            j_second = j;

                        }
                    }
                }

                // если пара символов находится в одной строке
                if (i_first == i_second)
                {
                    s1 = Convert.ToString(encriptionMatrixOut[i_first, j_first]);
                    s2 = Convert.ToString(encriptionMatrixIn[i_second, j_second]);
                }

                ///если пара символов находиться в разных столбцах и строках
                if (i_first != i_second)
                {

                    s1 = Convert.ToString(encriptionMatrixOut[i_first, j_second]);
                    s2 = Convert.ToString(encriptionMatrixIn[i_second, j_first]);
                }

                //записыавем результат кодирования
                encodetString = encodetString + s1 + s2;
                richTextBox3.Text = encodetString.ToLower();
            }
        }

        #endregion
        private void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);
            
            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine();
            message = "<TheEnd>";

            richTextBox3.Text = "Сокет соединяется с " + sender.RemoteEndPoint.ToString() + "\n";
            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);
            string str = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            richTextBox3.Text += "\nОтвет от сервера: " + Encoding.UTF8.GetString(bytes, 0, bytesRec);

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("<TheEnd>") == -1)
                SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessageFromSocket(11000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка..", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    } 
}

