using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>  
    public partial class MainWindow : Window
    {
      
        string postfix = "";
        string infix = "";
        string prefix = "";
        
        public MainWindow()
        {
            InitializeComponent();
        }
        //透過後序計算數值
        static bool EvaluatePostfix(string postfix, out double result)
        {
            result = 0.0;
            Stack<double> stack = new Stack<double>();
            StringBuilder currentNumber = new StringBuilder();

            foreach (char ch in postfix)
            {
                if (char.IsDigit(ch) || ch == '.')
                {
                    currentNumber.Append(ch);
                }
                else if (ch == ' ')
                {
                    if (currentNumber.Length > 0)
                    {
                        double number = double.Parse(currentNumber.ToString());
                        stack.Push(number);
                        currentNumber.Clear();
                    }
                }
                else if (IsOperator(ch))
                {
                    if (stack.Count < 2)
                    {
                        return false; // 運算元不足，無法計算
                    }

                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    double operationResult = ApplyOperator(operand1, operand2, ch);
                    stack.Push(operationResult);
                }
            }
            //只有單一個數字的時候
            if (currentNumber.Length > 0)
            {
                double number = double.Parse(currentNumber.ToString());
                stack.Push(number);
            }

            if (stack.Count != 1)
            {
                return false; // 堆疊中不正確，無法計算
            }

            result = stack.Pop();
            return true; // 成功計算
        }


        static double ApplyOperator(double operand1, double operand2, char op)
        {
            switch (op)
            {
                case '+':
                    return operand1 + operand2;
                case '-':
                    return operand1 - operand2;
                case '*':
                    return operand1 * operand2;
                case '/':
                    return operand1 / operand2;
                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

      

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            TextRes.Text += btn.Content.ToString();      
        }
       
        static bool convertIntopost(ref string infix, out string postfix)
        {
            postfix = "";
            Stack<char> s1 = new Stack<char>();

            for (int i = 0; i < infix.Length; i++)
            {
                char ch = infix[i];
                //是數字
                if (char.IsDigit(ch))
                {   //StringBuilder可以呈現多位數
                    StringBuilder operand = new StringBuilder();
                    //判斷是否為多位數
                    while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.'))
                    {
                        operand.Append(infix[i]);
                        i++;
                    }
                    postfix += operand.ToString() + " ";
                    i--;
                }
                //是符號
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                {
                    if (s1.Count <= 0)
                    {
                        s1.Push(ch);
                    }
                    else
                    {
                        int prio = 0;
                        if (s1.Peek() == '*' || s1.Peek() == '/')
                        {
                            prio = 1;
                        }

                        if (prio == 1)
                        {
                            if (ch == '+' || ch == '-')
                            {
                                postfix += s1.Pop() + " ";
                                i--;
                            }
                            else
                            {
                                postfix += s1.Pop() + " ";
                                i--;
                            }
                        }
                        else
                        {
                            if (ch == '+' || ch == '-')
                            {
                                while (s1.Count > 0 && (s1.Peek() == '+' || s1.Peek() == '-' || s1.Peek() == '*' || s1.Peek() == '/'))
                                {
                                    postfix += s1.Pop() + " ";
                                }
                                s1.Push(ch);
                            }
                            else
                            {
                                s1.Push(ch);
                            }
                        }
                    }
                }
                else if (ch == '(')
                {
                    s1.Push(ch);
                }
                else if (ch == ')')
                {
                    while (s1.Count > 0 && s1.Peek() != '(')
                    {
                        postfix += s1.Pop() + " ";
                    }
                    s1.Pop(); // Pop the '('
                }
            }
            //把剩下stack的東西pop
            while (s1.Count > 0)
            {
                postfix += s1.Pop() + " ";
            }

            return true;
        }

        static bool convertIntopre(ref string infix, out string prefix)
        {
            prefix = "";
            Stack<char> operators = new Stack<char>();
            Stack<string> operands = new Stack<string>();
            int i = infix.Length - 1; // 從後往前處理

            while (i >= 0)
            {
                char ch = infix[i];

                if (char.IsDigit(ch))
                {
                    StringBuilder operand = new StringBuilder();
                    while (i >= 0 && (char.IsDigit(infix[i]) || infix[i] == '.'))
                    {
                        operand.Insert(0, infix[i]); // 從前面插入字元
                        i--;
                    }
                    operands.Push(operand.ToString());
                    continue;
                }
                else if (ch == ' ')
                {
                    i--;
                    continue;
                }
                else if (IsOperator(ch))    
                {
                    while (operators.Count > 0 && Precedence(operators.Peek()) > Precedence(ch))
                    {
                        string operand1 = operands.Pop();
                        string operand2 = operands.Pop();
                        char op = operators.Pop();
                        operands.Push(op + operand1 + operand2);
                    }
                    operators.Push(ch);
                }
                else if (ch == ')')
                {
                    operators.Push(ch);
                }
                else if (ch == '(')
                {
                    while (operators.Count > 0 && operators.Peek() != ')')
                    {
                        string operand1 = operands.Pop();
                        string operand2 = operands.Pop();
                        char op = operators.Pop();
                        operands.Push(op + operand1 + operand2);
                    }
                    operators.Pop(); // Pop the ')'
                }

                i--;
            }

            while (operators.Count > 0)
            {
                string operand1 = operands.Pop();
                string operand2 = operands.Pop();
                char op = operators.Pop();
                operands.Push(op + operand1 + operand2);
            }

            prefix = operands.Pop();

            return true; // 成功轉換
        }




        static bool IsOperator(char ch)
        {
            return ch == '+' || ch == '-' || ch == '*' || ch == '/';
        }

        static int Precedence(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                default:
                    return 0;
            }
        }

        


        private void button_Clear(object sender, RoutedEventArgs e)
        {
          Decimal.Clear();
          TextRes.Clear();
          Binary.Clear();
          Postorder.Clear();
          Preorder.Clear();
        }

        

        private void button_Euals(object sender, RoutedEventArgs e)
        {
            //second = int.Parse(Decimal.Text);
            
           
            //infix to postfix
            infix = TextRes.Text;
            postfix = Postorder.Text;
            convertIntopost(ref infix, out postfix);
            Postorder.Text=postfix;
            //infix to prefix
            prefix = Preorder.Text;
            convertIntopre(ref infix, out prefix);
            Preorder.Text=prefix;
            //calculate
            EvaluatePostfix(postfix,out double a);

            Decimal.Text =a.ToString();




            //decimal to binary
            double remains = a - (int)a;           
            int b=Convert.ToInt32((int)a);
            string x = Convert.ToString(b, 2);
            if (remains != 0) 
            {
                 x = x+'.';
                for (int i = 0; i < 14; i++) 
                {
                    remains = remains * 2;

                    if (remains > 1)
                    {
                        x = x + '1';
                        remains -= 1;
                    }
                    else if (remains == 1)
                    {
                        x = x + '1';
                        break;
                    }
                    
                    else x = x + "0";
                }
            }           
            Binary.Text = x;          
        }

        //打開新視窗
        private void query_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
            
        }


        //Mysql insert

        public bool CheckIfDataExists(string name)
        {
            string connection = "datasource=localhost;port=3306;username=root;password=;database=calculatorrecords";
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM calrecords WHERE sentence = @name";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@name", name);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0; // 如果 count 大於 0，表示資料已存在
                }
            }
        }
       
        private void insert_Click(object sender, RoutedEventArgs e)
        {
            string connection = "datasource=localhost;port=3306;username=root;password=;database=calculatorrecords";
            string sentence = this.TextRes.Text;

            if (!CheckIfDataExists(sentence))
            {
                string query = "INSERT INTO calrecords(sentence,preorder,postorder,deci,bi) VALUES('" + sentence + "','" + this.Preorder.Text + "','" + this.Postorder.Text + "','" + this.Decimal.Text + "','" + this.Binary.Text + "')";

                using (MySqlConnection conn = new MySqlConnection(connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    
                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("資料插入成功！");
                        }
                        else
                        {
                            MessageBox.Show("無法插入資料。");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("發生錯誤：" + ex.Message);
                    }
                 
                }
            }
            else
            {
                MessageBox.Show("相同的資料已存在。");
            }
        }


    }




        //Mysql query


}

    