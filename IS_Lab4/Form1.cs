using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace IS_Lab4
{
    public partial class Form1 : Form
    {

        private string ToBinary(BigInteger s)
        {
            string res = "";
            Stack<BigInteger> stic = new Stack<BigInteger>();
            while (s > 0)
            {
                stic.Push(s % 2);
                s = s / 2;
            }
            while (stic.Count > 0)
            {
                res += stic.Pop();
            }
            return res;
        }
        private BigInteger fastPow(BigInteger a, BigInteger b, BigInteger n)
        {
            if (b == 0)
                return 1;
            BigInteger y;
            string code = ToBinary(b);
            string[] ai = new string[code.Length];
            ai[0] = a.ToString();
            BigInteger temp;
            for (int i = 1; i < code.Length; i++)
            {
                temp = BigInteger.Parse(ai[i - 1]);
                if (code[i] == '1')
                    ai[i] = ((temp * temp * a) % n).ToString();
                else
                    ai[i] = ((temp * temp) % n).ToString();
            }
            y = BigInteger.Parse(ai[ai.Length - 1]);
            return y;
        }
      
        private bool TestMilleraRabina(BigInteger n)
        {
            if (n == 2 || n == 3)
                return true;
            if (n % 2 == 0)
                return false;
            BigInteger s = 0;
            BigInteger u = n - 1;
            Random rnd = new Random();
            while (u % 2 == 0)
            {
                u = u / 2;
                s++;
            }
            BigInteger a;
            BigInteger b;
            byte[] bytes = new byte[n.ToByteArray().LongLength];

            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
            for (int i = 0; i < BigInteger.Log(n, 2); i++)
            {
                do
                {
                    rngCSP.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a > n - 2);
                b = fastPow(a, u, n);
                if (b == 1 || b == n - 1)
                    continue;
                else
                {
                    int j = 1;
                    while (j < s)
                    {
                        b = fastPow(b, 2, n);
                        if (b == n - 1)
                            break;
                        if (b == 1)
                            return false;
                        j++;
                    }
                    if (b != n - 1)
                        return false;
                }
            }
            return true;
        }
        private BigInteger GeneratePrime(int n) // передается бит
        {
            BigInteger res;
            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
            int length;
            if (n % 8 == 0)
                length = n / 8;
            else
                length = n / 8 + 1;
            byte[] bt = new byte[length];// уже байты
            do
            {
                rngCSP.GetBytes(bt);
                res = new BigInteger(bt);
            }
            while (res < 0 || res % 2 == 0);

            int t = Convert.ToInt32(label11.Text);
            while (TestMilleraRabina(res) == false)
            {
                res += 2;
                t++;
            }
            label11.Text=t.ToString();
            return res;
        }

        private BigInteger NOD(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            BigInteger x1 = 0, y1 = 0;

            BigInteger d = NOD(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        // е от 2 до n,  n (НОД(n,e)=1)
        public BigInteger Calculation_e(int l, BigInteger fi_n)
        {
            int k = (2 * l) / 3;
            if (k < 8)
                k = 8;  

            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();

            int countBytes;
            if (k % 8 == 0)
                countBytes = k / 8;
            else
                countBytes = k / 8 + 1;

            Byte[] arrayBytes = new Byte[countBytes];
            rngCSP.GetBytes(arrayBytes);

            BigInteger result = new BigInteger(arrayBytes);
            if (result < 0)
            {
                result = -result;
            }
            BigInteger x = 0, y = 0;
           
            while (NOD(result, fi_n, ref x, ref y) != 1)
            {
                result++;
            }
            return result;
        }


        // d из условия e*d mod fi_n=1
        public BigInteger Calculation_d(BigInteger fi_n, BigInteger E)
        {
            BigInteger x = 0, y = 0;
            NOD(fi_n, E, ref x, ref y);
            return y < 0 ? y + fi_n : y;
        }

 

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            
            label11.Text = "0";
            int length;
            
            if (Int32.TryParse(textBox1.Text, out length))
            {
                if(length <8)
                {
                    MessageBox.Show("Введите длину бит не менее 8");
                    textBox1.Text = "";
                    return;
                }
                textBox_P.Text = GeneratePrime(length).ToString();
                textBox_Q.Text = GeneratePrime(length).ToString();

                BigInteger p = BigInteger.Parse(textBox_P.Text);
                BigInteger q = BigInteger.Parse(textBox_Q.Text);

                BigInteger n = p * q;
                textBox_N.Text= n.ToString();

                BigInteger fN = (p-1)*(q-1);
                textBox_FN.Text = fN.ToString();

                BigInteger E = Calculation_e(length, fN);
                textBox_E.Text = E.ToString();

                BigInteger d = Calculation_d(fN, E);
                textBox_D.Text = d.ToString();
            }
            else
            {
                MessageBox.Show("Введите целое положительное число");
                textBox1.Text = "";
                return;

            }


        }

        private void cleanButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox_P.Text = "";
            textBox_Q.Text = "";
            textBox_N.Text = "";
            textBox_FN.Text = "";
            textBox_E.Text = "";
            textBox_D.Text = "";
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            label11.Text = "0";
        }

     

        private void cryptButton_Click(object sender, EventArgs e)
        {
            if (textBox_P.Text == "")
            {
                MessageBox.Show("Сгенерируйте ключи");
                return;
            }
            BigInteger E = BigInteger.Parse(textBox_E.Text);
            BigInteger N = BigInteger.Parse(textBox_N.Text);

            Durak(richTextBox1.Text);

            string k = "^%№(!@#$%^&*()_+=?/|\\~`.,')";
            foreach (char c in k)
            {
                foreach (char m in richTextBox1.Text)
                {
                    if (c == m)
                    {
                        MessageBox.Show("!!!!!!!!");
                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        return;
                    }
                    if (m == '"')
                    {
                        MessageBox.Show("??????!");
                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        return;
                    }
                }
            }

            string txt1 = richTextBox1.Text;

            BigInteger[] text = new BigInteger[txt1.Length];
            for (int i = 0; i < txt1.Length; i++)
            {
                if (radioButton1.Checked == true)
                {
                    text[i] = RusLetterToNumber( txt1[i]);                   
                }
                else
                {
                        text[i] = EngLetterToNumber(txt1[i]);
                }
            }


            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i == text.Length - 1)
                {
                    result = result + fastPow(text[i], E, N);
                }
                else
                {
                    result = result + fastPow(text[i], E, N) + " ";
                }
            }
            string[] x = result.Split(' ');

            BigInteger[] arrRes = new BigInteger[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                arrRes[i] = BigInteger.Parse(x[i]);
            }
            string code = "";
            for (int i = 0; i < arrRes.Length; i++)
            {
                if (i == arrRes.Length - 1)
                {
                    code = code + arrRes[i];
                }
                else
                    code = code + arrRes[i] + " ";
            }
            richTextBox2.Text = code;
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {
            if (textBox_P.Text == "")
            {
                MessageBox.Show("Сгенерируйте ключи");
                return;
            }
            BigInteger D = BigInteger.Parse(textBox_D.Text);
            BigInteger N = BigInteger.Parse(textBox_N.Text);

            Durak(richTextBox1.Text);
            string k = "^%№(!@#$%^&*()_+=?/|\\~`.,')";
            foreach (char c in k)
            {
                foreach (char m in richTextBox1.Text)
                {
                    if (c == m)
                    {
                        MessageBox.Show("!!!!!!!!");
                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        return;
                    }
                    if (m == '"')
                    {
                        MessageBox.Show("??????!");
                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        return;
                    }
                }
            }

            string encrText = richTextBox1.Text;
            string[] y = encrText.Split(' ');

            BigInteger[] arrY = new BigInteger[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                arrY[i] = BigInteger.Parse(y[i]);

            }
            string resY = "";
            for (int i = 0; i < arrY.Length; i++)
            {
                // c=h^d mod n, где с-это символ исходного сообщения 
                BigInteger c = fastPow(arrY[i], D, N);
                if (i == arrY.Length - 1)
                    resY = resY + c;
                else
                    resY = resY + c + " ";
            }
            string [] resArr = resY.Split();
            string [] res = new string[resArr.Length];

            for (int i = 0; i < resArr.Length; i++)
            {
                BigInteger t = BigInteger.Parse(resArr[i]);


                if (radioButton1.Checked == true)
                {
                    if (t > 45)
                    {
                        t = t % 45;
                    }
                    res[i] += NumberToRusLetter(t);
                }
                else
                {
                    if (t > 37)
                    {
                        t = t % 37;
                    }
                    res[i] += NumberToEngLetter(t);
                }

            }
            string result = "";
            foreach(string c in res)
            {
                result += c;
            }
            richTextBox2.Text = result.ToString();


        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true) { radioButton2.Checked = false; }
            if (radioButton2.Checked == true) { radioButton1.Checked = false; }
        }

        private void Durak(string t)
        {
            if (radioButton2.Checked == false && radioButton1.Checked == false)
            {
                MessageBox.Show("Выберите язык!");
                return;
            }
            if (radioButton1.Checked == true)
            {
                foreach (char c in t)
                {
                    if (c >= 'a' && c <= 'z')
                    {
                        MessageBox.Show("выберите другой язык:)!");
                        richTextBox1.Text = "";
                        radioButton1.Checked = false;
                        return;
                    }
                }
                foreach (char c in t)
                {
                    if (c >= 'a' && c <= 'z')
                    {
                        MessageBox.Show("Язык ключа должен совпадатьс языком текста:)!");
                        textBox1.Text = "";
                        return;
                    }
                }
            }
            if (radioButton2.Checked == true)
            {
                foreach (char c in t)
                {
                    if (c >= 'а' && c <= 'я' || c == 'ё')
                    {
                        MessageBox.Show("выберите другой язык:)!");
                        richTextBox1.Text = "";
                        radioButton2.Checked = false;
                        return;
                    }
                }
                foreach (char c in t)
                {
                    if (c >= 'а' && c <= 'я' || c == 'ё')
                    {
                        MessageBox.Show("Язык ключа должен совпадатьс языком текста:)!");
                        textBox1.Text = "";
                        return;
                    }
                }
            }

            //string k = "^%№(!@#$%^&*()_+=?/|\\~`.,')";
            //foreach (char c in k)
            //{
            //    foreach (char m in t)
            //    {
            //        if (c == m)
            //        {
            //            MessageBox.Show("!!!!!!!!");
            //            richTextBox1.Text = "";
            //            richTextBox2.Text = "";
            //            return;
            //        }
            //        if (m == '"')
            //        {
            //            MessageBox.Show("??????!");
            //            richTextBox1.Text = "";
            //            richTextBox2.Text = "";
            //            return;
            //        }
            //    }
            //}
           
        }

       

        public static BigInteger RusLetterToNumber(char c)
        {
            if (c == ' ') return ' ';
            if (c == 'ё') return 7;
            if (c == 'а') return 44;
            if (c == 'ю') return 45;
            else
            {
                if (c == '0') return 34; if (c == '1') return 35; if (c == '2') return 36; if (c == '3') return 37; if (c == '4') return 38; if (c == '5') return 39; if (c == '6') return 40; if (c == '7') return 41; if (c == '8') return 42; if (c == '9') return 43;
                BigInteger tmp = (int)c - 1071;//a=1072 //48-0
                if (tmp > 6 && tmp < 34)
                    return tmp + 1;
                else
                    return tmp;
            }
        }
        public static char NumberToRusLetter(BigInteger n)
        {
            if (n == 44) return 'а';
            if (n == 45) return 'ю';
            if (n == 32) return ' ';
            if (n >= 34) return (char)(n + 14);
            if (n == 0) return '9';// при подсчете остатка от деления
            if (n == 7) return 'ё';
            else
            {
                if (n > 6) return (char)(n + 1070);
                else return (char)(n + 1071);
            }

        }

        public static BigInteger EngLetterToNumber(char c)
        {
            if (c == ' ') return ' ';
            if (c == 'a') return 37;
            if (c == '0') return 27; if (c == '1') return 28; if (c == '2') return 29; if (c == '3') return 30; if (c == '4') return 31; if (c == '5') return 45; if (c == '6') return 33; if (c == '7') return 34; if (c == '8') return 35; if (c == '9') return 36;
            BigInteger tmp = (int)c - 96; //a=97
            return tmp;
        }
        
        public static char NumberToEngLetter(BigInteger n)
        {
            if (n == 37) return 'a';
            if (n == 32) return ' ';
            if (n == 45) return '5';
            if (n >= 27) return (char)(n + 21);
            if (n == 0) return '9';
            else return (char)(n + 96);

        }

    }
}
