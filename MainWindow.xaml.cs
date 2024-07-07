using MailKit.Security;
using MailKit;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Restaurant_Manager.DAL;
using Restaurant_Manager.Models;
using Restaurant_Manager.CustomerPannle;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MailKit.Net.Smtp;
using static System.Net.WebRequestMethods;

namespace Restaurant_Manager
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _emailID;
        public string EmailID
        {
            get => _emailID;
            set
            {
                _emailID = value;
                OnPropertyChange(nameof(_emailID));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChange(nameof(_name));
            }
        }

        private string _number;
        public string Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChange(nameof(_number));
            }
        }
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChange(nameof(_username));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MainWindow : Window
    {
        static int otp = 0;
        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
        public static void SendMessage(string email)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("salirezamir", "salirezamir@yandex.com"));
            mimeMessage.To.Add(new MailboxAddress("DEV TEST OTP", email));
            mimeMessage.Subject = "DEV TEST OTP";
            mimeMessage.Body = new TextPart("plain")
            {
                Text = "***DEV TEST OTP***\nOTP is : " + otp.ToString()
            };
            using (var client = new SmtpClient(new ProtocolLogger("smtp.log")))
            {
                client.Connect("smtp.yandex.com", 465, SecureSocketOptions.SslOnConnect);

                client.Authenticate("salirezamir", "gtvmchybehgvlusz");

                client.Send(mimeMessage);

                client.Disconnect(true);
            }
        }
        private readonly RestaurantContext _context = new RestaurantContext();
        public MainWindow()
        {
            try
            {
                _context.Database.EnsureCreated();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameInTx.Text) || string.IsNullOrEmpty(passwordInTx.Password))
                return;
            if (Validation.GetErrors(usernameInTx).Count > 0 || Validation.GetErrors(passwordInTx).Count > 0)
                return;
            User? user = _context.Users.FirstOrDefault(u => u.Username == usernameInTx.Text && u.Password == CreateMD5(passwordInTx.Password));
            if (user != null)
            {
                MessageBox.Show("Login Successful");
                if (user.Type == User.Types.Admin)
                {
                    AdminWindow adminWindow = new AdminWindow(user);
                    adminWindow.Show();
                    this.Close();
                }

                else if (user.Type == User.Types.Customer)
                {
                    CustomerPanel customerPanel = new CustomerPanel(user);
                    customerPanel.Show();
                    this.Close();
                }

                else if (user.Type == User.Types.Restaurant)
                {
                    Restaurant restaurant = _context.Users.Where(x => x.Id == user.Id).Select(x => x.Restaurant).FirstOrDefault();
                    RestaurantWindow resturantWindow = new RestaurantWindow(restaurant, user);
                    resturantWindow.Show();
                    this.Close();
                }


            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }


        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordUpTx.Password) || string.IsNullOrEmpty(usernameUpTx.Text)
                || string.IsNullOrEmpty(emailTx.Text) || string.IsNullOrEmpty(phoneTx.Text))
                return;
            if (Validation.GetErrors(usernameUpTx).Count > 0 || Validation.GetErrors(emailTx).Count > 0
                || Validation.GetErrors(phoneTx).Count > 0 || Validation.GetErrors(passwordUpTx).Count > 0)
                return;
            if (passwordUpTx.Password != passwordUp2Tx.Password)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }
            Random random = new Random();
            otp = random.Next(10000, 99999);
            SendMessage(emailTx.Text);
            MessageBox.Show(otp.ToString());
            MessageBox.Show("Email Sent");
            otpGrid.Visibility = Visibility.Visible;
            usernameUpTx.IsEnabled = false;
            passwordUpTx.IsEnabled = false;
            passwordUp2Tx.IsEnabled = false;
            emailTx.IsEnabled = false;
            phoneTx.IsEnabled = false;
            nameTx.IsEnabled = false;
        }

        private void VerifyOTPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(otpTx.Text,out int otptx))
            {
                if (otptx == otp)
                {
                    User user = new User
                    {
                        Username = usernameUpTx.Text,
                        Password = CreateMD5(passwordUpTx.Password),
                        Email = emailTx.Text,
                        Phone = phoneTx.Text,
                        Type = User.Types.Customer,
                        Name = nameTx.Text
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    Cleaner();
                    MessageBox.Show("User Created");
                }
                else
                {
                    MessageBox.Show("Invalid OTP");
                }
            }
        }

        private void ResendOTPBtn_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            otp = random.Next(10000, 99999);
            SendMessage(emailTx.Text);
            MessageBox.Show("Email Sent");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cleaner();
        }

        private void Cleaner()
        {
            otpGrid.Visibility = Visibility.Hidden;
            usernameInTx.Clear();
            usernameUpTx.Clear();
            passwordInTx.Clear();
            passwordUpTx.Clear();
            passwordUp2Tx.Clear();
            nameTx.Clear();
            emailTx.Clear();
            phoneTx.Clear();
            otpTx.Clear();
            usernameUpTx.IsEnabled = true;
            passwordUpTx.IsEnabled = true;
            passwordUp2Tx.IsEnabled = true;
            emailTx.IsEnabled = true;
            phoneTx.IsEnabled = true;
            nameTx.IsEnabled = true;
        }
    }
}