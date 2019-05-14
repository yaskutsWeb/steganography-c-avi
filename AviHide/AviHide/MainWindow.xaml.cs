using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AviHide
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			#region CHANGE_LOCATIONS
			M_ENCRYPT.IsChecked = true;
			m1.Visibility = Visibility.Visible;
			textBox_newkey.Visibility = Visibility.Collapsed;
			Label_key.Visibility = Visibility.Collapsed;
			m2.Visibility = Visibility.Collapsed;
			m3.Visibility = Visibility.Collapsed;
			#endregion
		}

		private void M_ENCRYPT_Checked(object sender, RoutedEventArgs e)
		{
			#region CHANGE_LOCATIONS
			M_DECRYPT.IsChecked = false;
			M_HELP.IsChecked = false;
			m1.Visibility = Visibility.Visible;
			m2.Visibility = Visibility.Collapsed;
			m3.Visibility = Visibility.Collapsed;
			#endregion
		}

		private void M_DECRYPT_Checked(object sender, RoutedEventArgs e)
		{
			#region CHANGE_LOCATIONS
			M_ENCRYPT.IsChecked = false;
			M_HELP.IsChecked = false;
			m1.Visibility = Visibility.Collapsed;
			m2.Visibility = Visibility.Visible;
			m3.Visibility = Visibility.Collapsed;
			textBox_newkey.Visibility = Visibility.Collapsed;
			Label_key.Visibility = Visibility.Collapsed;
			#endregion
		}

		private void M_HELP_Checked(object sender, RoutedEventArgs e)
		{
			#region CHANGE_LOCATIONS
			M_ENCRYPT.IsChecked = false;
			M_DECRYPT.IsChecked = false;
			m1.Visibility = Visibility.Collapsed;
			m2.Visibility = Visibility.Collapsed;
			m3.Visibility = Visibility.Visible;
			textBox_newkey.Visibility = Visibility.Collapsed;
			Label_key.Visibility = Visibility.Collapsed;
			#endregion
		}

		// кнопка выбора видео 
		private void buttonOpenSourceVideo_1_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".avi"; 
			dlg.Filter = "AVI Videos|*.avi"; 
			Nullable<bool> result = dlg.ShowDialog();
			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				textBoxSourceVideo_1.Text = filename;
			}
		}

		// кнопка выбора сообщения
		private void buttonOpenSourceMessage_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".txt"; 
			dlg.Filter = "Documents|*.*"; 
			Nullable<bool> result = dlg.ShowDialog();

			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				textBoxSourceMessage.Text = filename;
			}
		}

		// функция для вывода ошибок
		private void alert(string message)
		{
			string messageBoxText = message;
			string caption = "AVIHide";
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Error;
			MessageBox.Show(messageBoxText, caption, button, icon);
		}

		// кнопка зашифровать
		private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
		{
			string sourceMessageFileName = textBoxSourceMessage.Text;
			string sourceVideoFileName = textBoxSourceVideo_1.Text;
			string key = textBoxKey_1.Text;
			int LSB = Convert.ToBoolean(radioButton1bit.IsChecked) ? 1 : 2;

			if (sourceMessageFileName.Length == 0)
			{
				alert("Выберите текстовый файл!");
			}
			else if (sourceVideoFileName.Length == 0)
			{
				alert("Выберите видеофайл!");
			}
			else if (key.Length == 0)
			{
				alert("Введите ключ!");
			}
			else if (!File.Exists(sourceMessageFileName))
			{
				alert("Текстовый файл не найден!");
			}
			else if (!File.Exists(sourceVideoFileName))
			{
				alert("Видеофайл не найден!");
			}
			else
			{
				var dlg = new SaveFileDialog();
				dlg.DefaultExt = ".avi"; 
				dlg.Filter = "AVI Videos|*.avi"; 
				dlg.Title = "Выберите видео, в которое будет записана информация";

				Nullable<bool> result = dlg.ShowDialog();

				if (result == true)
				{
					// Everything valid here. Begin with encryption.
					int pasha = 0;
					// Fill Engine with variables.
					Engine.SourceMessageFileName = sourceMessageFileName;
					Engine.SourceVideoFileName = sourceVideoFileName;
					Engine.Key = key;
					Engine.LsbMode = LSB;
					Engine.OutputVideoFileName = dlg.FileName;
					MessageBox.Show("Пожалуйста подождите, идет шифрование сообщения");
					Engine.EncryptAndSave();
					MessageBox.Show("Шифрование завершено!");
					Label_key.Visibility = Visibility.Visible;
					textBox_newkey.Visibility = Visibility.Visible;
					textBox_newkey.Text = Engine.newkey;
				}
				else
				{
					alert("Новый видеофайл не указан! Отмена операции.");
				}
			}
		}

		private void buttonOpenSourceVideo_2_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".avi";
			dlg.Filter = "AVI Videos|*.avi";
			Nullable<bool> result = dlg.ShowDialog();
			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				textBoxSourceVideo_2.Text = filename;
			}
		}

		private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
		{
			string sourceMessageFileName = textBoxSourceMessage.Text;
			string sourceVideoFileName = textBoxSourceVideo_2.Text;
			string key = textBoxKey_2.Text;
			int LSB = Convert.ToBoolean(radioButton1bit.IsChecked) ? 1 : 2;

			if (sourceVideoFileName.Length == 0)
			{
				alert("Выберите видео!");
			}
			else if (key.Length == 0)
			{
				alert("Введите ключ!");
			}
			else if (!File.Exists(sourceVideoFileName))
			{
				alert("Видеофайл не найден!");
			}
			else if (key.Length!=4)
			{
				alert("Введите ключ нужного формата!");
			}
			else
			{
				var dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt"; 
				dlg.Filter = "All Files (*.*)|*.*"; 
				dlg.Title = "Выберите текстовый файл, куда будет записана спрятанная информация";

				Nullable<bool> result = dlg.ShowDialog();

				if (result == true)
				{
					Engine.SourceVideoFileName = sourceVideoFileName;
					Engine.Key = key;
					Engine.LsbMode = LSB;

					Engine.OutputMessageFileName = dlg.FileName;
					Engine.DecryptAndSave();
				}
				else
				{
					alert("Выходной файл не найден! Отмена операции.");
				}
			}
		}
	}
}