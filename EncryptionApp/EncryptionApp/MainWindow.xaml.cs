/*
    Encryption Application
    Copyright (C) 2024 Cam Cu Thanh
    Contributions by Cam Cu Thanh - <https://github.com/cuthanhcam>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EncryptionApp
{
    public partial class MainWindow : Window
    {
        private TextBox txtKeyVisible; // TextBox hiển thị key khi checkbox được bật
        public MainWindow()
        {
            InitializeComponent();

            // Tạo TextBox để hiển thị key (ẩn mặc định)
            txtKeyVisible = new TextBox
            {
                Width = txtKey.Width,
                Height = txtKey.Height,
                Margin = txtKey.Margin,
                HorizontalAlignment = txtKey.HorizontalAlignment,
                VerticalAlignment = txtKey.VerticalAlignment,
                Visibility = Visibility.Hidden // Ban đầu ẩn TextBox
            };

            // Thêm TextBox hiển thị key vào Grid
            (this.Content as Grid).Children.Add(txtKeyVisible);
        }

        // MD5 Hash function
        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string Encrypt3DES(string plaintext, string key)
        {
            // Convert plaintext to byte array
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // Convert key to byte array
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Initialize TripleDES encryption provider
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                // Set key and IV (Initialization Vector)
                tdes.Key = keyBytes;
                tdes.Mode = CipherMode.ECB; // Use ECB mode for simplicity (or use CBC for better security with IV)
                tdes.Padding = PaddingMode.PKCS7;

                // Encrypt the data
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextBytes, 0, plaintextBytes.Length);
                        cs.FlushFinalBlock();
                        byte[] encryptedBytes = ms.ToArray();

                        // Convert encrypted bytes to Base64 string for easier display/storage
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
        }

        // Hiển thị key dưới dạng văn bản thường khi checkbox được bật
        private void ShowKey_Checked(object sender, RoutedEventArgs e)
        {
            txtKeyVisible.Text = txtKey.Password; // Sao chép nội dung từ PasswordBox sang TextBox
            txtKeyVisible.Visibility = Visibility.Visible; // Hiển thị TextBox
            txtKey.Visibility = Visibility.Hidden; // Ẩn PasswordBox
        }

        // Ẩn key dưới dạng password khi checkbox bị tắt
        private void ShowKey_Unchecked(object sender, RoutedEventArgs e)
        {
            txtKey.Password = txtKeyVisible.Text; // Sao chép nội dung từ TextBox về PasswordBox
            txtKeyVisible.Visibility = Visibility.Hidden; // Ẩn TextBox
            txtKey.Visibility = Visibility.Visible; // Hiển thị PasswordBox
        }

        private void RegisterUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Validate password
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự, bao gồm một chữ hoa, một số và một ký tự đặc biệt.");
                return;
            }

            string hash = ComputeMD5Hash(username + password);
            File.WriteAllText(@"D:\user.txt", hash);
            MessageBox.Show("Đăng ký thành công!");
        }

        // Function to validate password
        private bool IsValidPassword(string password)
        {
            // At least 8 characters, including one uppercase letter, one number, and one special character.
            string pattern = @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+{}\[\]:;'"",.<>/?\\|`~\-]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        // Login user
        private void LoginUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Check if the user file exists
            if (!File.Exists(@"D:\user.txt"))
            {
                MessageBox.Show("User chưa tồn tại. Vui lòng đăng ký trước.");
                return;
            }

            // Compute the hash for the entered username and password
            string enteredHash = ComputeMD5Hash(username + password);

            // Read the stored hash from user.txt
            string storedHash = File.ReadAllText(@"D:\user.txt");

            // Compare hashes
            if (enteredHash == storedHash)
            {
                MessageBox.Show("Đăng nhập thành công!");
            }
            else
            {
                MessageBox.Show("Thông tin đăng nhập không đúng.");
            }
        }


        // Sign Plaintext with MD5 hash
        private void SignPlainText_Click(object sender, RoutedEventArgs e)
        {
            string plaintext = txtPlaintext.Text;
            string userHash = File.ReadAllText(@"D:\user.txt");
            txtPlaintext.Text = plaintext + "_" + userHash;
            MessageBox.Show("Ký tên thành công!");
        }

        // Show Plaintext
        private void ShowPlainText_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(txtPlaintext.Text);
        }

        // Encrypt text with 3DES
        public static string EncryptText(string plainText, string key)
        {
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = Encoding.UTF8.GetBytes(key.Substring(0, 16));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = tdes.CreateEncryptor();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        private void EncryptText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string plaintext = txtPlaintext.Text;
                string key = txtKey.Password; // Sử dụng PasswordBox để lấy key nếu checkbox chưa được chọn

                // Validate key size for 3DES (should be 24 characters)
                if (key.Length == 16)
                {
                    key += key.Substring(0, 8); // Pad key để đạt 24 ký tự
                }
                else if (key.Length != 24)
                {
                    MessageBox.Show("Key must be 16 or 24 characters long for 3DES encryption.");
                    return;
                }

                // Perform encryption
                string ciphertext = Encrypt3DES(plaintext, key);
                txtCiphertext.Text = ciphertext;

                MessageBox.Show("Văn bản đã được mã hoá thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Decrypt text with 3DES
        public static string DecryptText(string cipherText, string key)
        {
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = Encoding.UTF8.GetBytes(key.Substring(0, 16));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = tdes.CreateDecryptor();
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private void DecryptText_Click(object sender, RoutedEventArgs e)
        {
            string ciphertext = txtCiphertext.Text;
            string key = txtKey.Password; // Sử dụng PasswordBox để lấy key nếu checkbox chưa được chọn

            if (key.Length < 16)
            {
                MessageBox.Show("Key must be at least 16 characters long!");
                return;
            }

            string plaintext = DecryptText(ciphertext, key);
            txtPlaintext.Text = plaintext;
            MessageBox.Show("Giải mã thành công!");
        }

        // Save ciphertext to file
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            string ciphertext = txtCiphertext.Text;
            File.WriteAllText(@"D:\ciphertext.txt", ciphertext);
            MessageBox.Show("Ghi file thành công!");
        }

    }
}