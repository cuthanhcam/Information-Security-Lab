using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;


namespace RSAEncryptionApp
{
    public partial class MainWindow : Window
    {
        private RSACryptoServiceProvider rsa;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int keySize = int.Parse(((ComboBoxItem)EncryptionModeComboBox.SelectedItem).Content.ToString());
                rsa = new RSACryptoServiceProvider(keySize);

                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                PublicKeyTextBox.Text = publicKey;
                PrivateKeyTextBox.Text = privateKey;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during key generation: " + ex.Message);
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rsa == null)
                {
                    MessageBox.Show("Please generate a key pair first.");
                    return;
                }

                string plaintext = InputTextBox.Text;
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                byte[] encryptedBytes = rsa.Encrypt(plaintextBytes, false);
                OutputTextBox.Text = Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during encryption: " + ex.Message);
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rsa == null)
                {
                    MessageBox.Show("Please generate a key pair first.");
                    return;
                }

                string ciphertext = InputTextBox.Text;
                byte[] encryptedBytes = Convert.FromBase64String(ciphertext);
                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
                OutputTextBox.Text = Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during decryption: " + ex.Message);
            }
        }
+       private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ciphertext = OutputTextBox.Text;

                if (string.IsNullOrWhiteSpace(ciphertext))
                {
                    MessageBox.Show("No ciphertext to save.");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = "ciphertext.txt",
                    DefaultExt = ".txt",        
                    Filter = "Text documents (.txt)|*.txt"
                };

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, ciphertext);
                    MessageBox.Show("Ciphertext saved to file: " + saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }
        }

    }
}
