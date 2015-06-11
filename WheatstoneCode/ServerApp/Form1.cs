using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private int connectedClients = 0;
        private delegate void WriteMessageDelegate(string msg);

        public Form1()
        {
            InitializeComponent();
            Server();
        }
        //матрица алфавита шифрования
        public string[,] encriptionMatrixOut =
                                         {
                                         {"А", "Ч", "Б", "М", "Ц", "В"}, //первая строка матрицы
                                         {"Ь", "Г", "Н", "Ш", "Д", "О"}, //вторая строка матрицы
                                         {"Е", "Щ", ",", "Х", "У", "П"}, //третья строка матрицы
                                         {".", "З", "Ъ", "Р", "И", "Й"}, //четвертая строка матрицы
                                         {"С", "-", "К", "Э", "Т", "Л"}, //пятая строка матрицы
                                         {"Ю", "Я", " ", "Ы", "Ф", "Ж"}  //шестая строка матрицы
                                         };
        private string[,] encriptionMatrixIn = new string[6, 6];

        private string alfavit = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ .-,";
        private string keyText;
        private string text; //исходный текст для шифрования

        private int i_first = 0, j_first = 0;  //координаты первого символа пары
        private int i_second = 0, j_second = 0;//координаты второго символа пары
        private string s1 = "", s2 = ""; //строки для хранения зашифрованного символа 
        private string encodetString; //зашифрованая строка
        private string decodetString; //расшифрованная строка

        private void Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Loopback, 3000); // Change to IPAddress.Any for internet wide Communication
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true) // Never ends until the Server is closed.
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                connectedClients++; // Increment the number of clients that have communicated with us.
                lblNumberOfConnections.Text = connectedClients.ToString();

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    connectedClients--;
                    lblNumberOfConnections.Text = connectedClients.ToString();
                    break;
                }

                //message has successfully been received
                UTF8Encoding encoder = new UTF8Encoding();

                // Convert the Bytes received to a string and display it on the Server Screen
                string msg = encoder.GetString(message, 0, bytesRead);
                WriteMessage(msg);

                // Now Echo the message back

                Echo("Сообщение успешно получено!", encoder, clientStream);
            }

            //tcpClient.Close();
        }

        private void WriteMessage(string msg)
        {
            if (this.rtbServer.InvokeRequired)
            {
                WriteMessageDelegate d = new WriteMessageDelegate(WriteMessage);
                //this.rtbServer.Invoke(d, new object[] { msg });
                richTextBox4.Text = msg;
            }
            else
            {
                this.rtbServer.AppendText(msg + Environment.NewLine);
            }
        }

        /// <summary>
        /// Echo the message back to the sending client
        /// </summary>
        /// <param name="msg">
        /// String: The Message to send back
        /// </param>
        /// <param name="encoder">
        /// Our UTF8Encoding
        /// </param>
        /// <param name="clientStream">
        /// The Client to communicate to
        /// </param>
        private void Echo(string msg, UTF8Encoding encoder, NetworkStream clientStream)
        {
            // Now Echo the message back
            byte[] buffer = encoder.GetBytes(msg);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        private void CreateOutMas()
        {
            alfavit = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ .-,";
            keyText = Convert.ToString(textBox1.Text).ToUpper();
            int i, j;
            // Создание второго массива с помощью ключа.
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 6; j++)
                {
                    if (keyText.Length > 0)
                    {
                        encriptionMatrixIn[i, j] = Convert.ToString(keyText[0]);
                        alfavit = alfavit.Replace(Convert.ToString(keyText[0]), "");
                        keyText = keyText.Replace(Convert.ToString(keyText[0]), "");
                    }
                    else
                    {
                        if (alfavit.Length > 0)
                        {
                            encriptionMatrixIn[i, j] = Convert.ToString(alfavit[0]);
                            alfavit = alfavit.Replace(Convert.ToString(alfavit[0]), "");
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateOutMas();
            text = "";
            encodetString = "";
            int i, j;
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
                richTextBox3.Text = "";
                richTextBox3.Text = encodetString.ToLower();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.tcpListener.Stop();
            this.listenThread.Abort();
        }
    }
}
