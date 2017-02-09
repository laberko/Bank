using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BankWPF.BankService;

namespace BankWPF
{
    public partial class MainWindow : Window
    {
        private readonly WcfServiceClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new WcfServiceClient();
            RefreshAsync();
        }

        private async void RefreshAsync()
        {
            try
            {
                var atm = await Task.Run(() => _client.GetAtmState());
                textBlock100.Text = atm.Banknote100.ToString();
                textBlock500.Text = atm.Banknote500.ToString();
                textBlock1000.Text = atm.Banknote1000.ToString();
                textBlock5000.Text = atm.Banknote5000.ToString();
                textBlockAccount.Text = atm.AccountValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void putButton_Click(object sender, RoutedEventArgs e)
        {
            int banknote100;
            int banknote500;
            int banknote1000;
            int banknote5000;
            if (!int.TryParse(textBox100.Text, out banknote100)
                || !int.TryParse(textBox500.Text, out banknote500)
                || !int.TryParse(textBox1000.Text, out banknote1000)
                || !int.TryParse(textBox5000.Text, out banknote5000)
                || banknote100*100 + banknote500*500 + banknote1000*1000 + banknote5000*5000 == 0
                || banknote100 < 0 || banknote500 < 0 || banknote1000 < 0 || banknote5000 < 0)
            {
                MessageBox.Show(this, "Неправильные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    _client.PutMoneyAsync(new Transaction
                    {
                        Banknote100 = banknote100,
                        Banknote500 = banknote500,
                        Banknote1000 = banknote1000,
                        Banknote5000 = banknote5000
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                RefreshAsync();
            }
            textBox100.Text = "0";
            textBox500.Text = "0";
            textBox1000.Text = "0";
            textBox5000.Text = "0";
        }

        private async void getButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var atm = await Task.Run(() => _client.GetAtmState());
                if (atm.AccountValue < 100)
                {
                    MessageBox.Show(this, "Сначала пополните счет!", "Ошибка!", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    getTextBox.Text = "0";
                    return;
                }
                int value;
                if (!int.TryParse(getTextBox.Text, out value) || value < 100)
                    MessageBox.Show(this, "Неправильные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    var transaction = await _client.GetMoneyAsync(value, false);
                    var bankValue = transaction.Banknote100*100 + 
                                    transaction.Banknote500*500 +
                                    transaction.Banknote1000*1000 +
                                    transaction.Banknote5000*5000;
                    var sb = new StringBuilder();
                    sb.AppendLine("Банкомат может выдать " + bankValue + " следующими банкнотами:");
                    sb.AppendLine("100: " + transaction.Banknote100 + " шт.");
                    sb.AppendLine("500: " + transaction.Banknote500 + " шт.");
                    sb.AppendLine("1000: " + transaction.Banknote1000 + " шт.");
                    sb.AppendLine("5000: " + transaction.Banknote5000 + " шт.");
                    sb.AppendLine("Продолжить?");
                    var result = MessageBox.Show(this, sb.ToString(), "Выдача наличных", MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        await _client.GetMoneyAsync(value, true);
                        RefreshAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            getTextBox.Text = "0";
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAsync();
        }

        private async void historyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = await Task.Run(() => _client.GetTransactions(10));
                var sb = new StringBuilder();
                sb.AppendLine("Последние 10 транзакций:");
                foreach (var t in list)
                {
                    var time = t.TimeStamp?.ToString("G") ?? string.Empty;
                    var value = t.Banknote100*100 + t.Banknote500*500 + t.Banknote1000*1000 + t.Banknote5000*5000;
                    var action = t.IsIncome ? "Внесение " : "Выдача ";
                    sb.AppendLine(action + "\t" + value + "\t" + time);
                }
                MessageBox.Show(this, sb.ToString(), "Последние операции", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
