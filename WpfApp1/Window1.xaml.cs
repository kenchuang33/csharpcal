using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;



namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            string connection = "datasource=localhost;port=3306;username=root;password=;database=calculatorrecords";
            string query = "SELECT * FROM calrecords";
            MySqlConnection conn = new MySqlConnection(connection);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd; 
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGrid.ItemsSource= dt.DefaultView;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string connection = "datasource=localhost;port=3306;username=root;password=;database=calculatorrecords";
            string query = "DELETE  FROM calrecords";
            MySqlConnection conn = new MySqlConnection(connection);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Open();
            int dr = cmd.ExecuteNonQuery();
            System.Windows.MessageBox.Show($"{dr} 行資料已刪除。");
        }
    }
   
}
