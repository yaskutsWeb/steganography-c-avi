using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviHide
{
	public class DES
	{
		private const int sizeOfBlock = 128; //в DES размер блока 64 бит, но поскольку в unicode символ в два раза длинее, то увеличим блок тоже в два раза
		private const int sizeOfChar = 16; //размер одного символа (in Unicode 16 bit)
		private const int shiftKey = 2; //сдвиг ключа 
		private const int quantityOfRounds = 16; //количество раундов
		string[] Blocks; //сами блоки в двоичном формате
		string[] check;

		/*Метод, доводящий строку до такого размера, чтобы она делилась на sizeOfBlock. 
		 * Размер увеличивается с помощью добавления к исходной строке символа “решетка”.*/
		private string StringToRightLength(string input)
		{
			while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
				input += "#";

			return input;
		}

		//Метод, разбивающий строку в обычном (символьном) формате на блоки.
		private void CutStringIntoBlocks(string input)
		{
			Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

			int lengthOfBlock = input.Length / Blocks.Length;

			for (int i = 0; i < Blocks.Length; i++)
			{
				Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
				Blocks[i] = StringToBinaryFormat(Blocks[i]);
			}
		}

		//Метод, разбивающий строку в двоичном формате на блоки.
		private void CutBinaryStringIntoBlocks(string input)
		{
			Blocks = new string[input.Length / sizeOfBlock];

			int lengthOfBlock = input.Length / Blocks.Length;

			for (int i = 0; i < Blocks.Length; i++)
				Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
		}

		//Метод, переводящий строку в двоичный формат.
		private string StringToBinaryFormat(string input)
		{
			string output = "";

			for (int i = 0; i < input.Length; i++)
			{
				string char_binary = Convert.ToString(input[i], 2);

				while (char_binary.Length < sizeOfChar)
					char_binary = "0" + char_binary;

				output += char_binary;
			}

			return output;
		}

		//Метод, доводящий длину ключа до нужной длины.
		private string CorrectKeyWord(string input, int lengthKey)
		{
			if (input.Length > lengthKey)
				input = input.Substring(0, lengthKey);
			else
				while (input.Length < lengthKey)
					input = "0" + input;

			return input;
		}

		//Один раунд шифрования алгоритмом DES.
		private string EncodeDES_One_Round(string input, string key)
		{
			string L = input.Substring(0, input.Length / 2);
			string R = input.Substring(input.Length / 2, input.Length / 2);

			return (R + XOR(L, f(R, key)));
		}

		//Один раунд дешифрования алгоритмом DES.
		private string DecodeDES_One_Round(string input, string key)
		{
			string L = input.Substring(0, input.Length / 2);
			string R = input.Substring(input.Length / 2, input.Length / 2);

			return (XOR(f(L, key), R) + L);
		}

		//XOR двух строк с двоичными данными.
		private string XOR(string s1, string s2)
		{
			string result = "";

			for (int i = 0; i < s1.Length; i++)
			{
				bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
				bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

				if (a ^ b)
					result += "1";
				else
					result += "0";
			}
			return result;
		}

		//Шифрующая функция f.
		private string f(string s1, string s2)
		{
			return XOR(s1, s2);
		}

		//Вычисление ключа для следующего раунда шифрования DES. Циклический сдвиг >> shiftKey.
		private string KeyToNextRound(string key)
		{
			for (int i = 0; i < shiftKey; i++)
			{
				key = key[key.Length - 1] + key;
				key = key.Remove(key.Length - 1);
			}

			return key;
		}

		//Вычисление ключа для следующего раунда расшифровки DES. циклический сдвиг << shiftKey.
		private string KeyToPrevRound(string key)
		{
			for (int i = 0; i < shiftKey; i++)
			{
				key = key + key[0];
				key = key.Remove(0, 1);
			}

			return key;
		}

		//Метод, переводящий строку с двоичными данными в символьный формат.
		private string StringFromBinaryToNormalFormat(string input)
		{
			string output = "";

			while (input.Length > 0)
			{
				string char_binary = input.Substring(0, sizeOfChar);
				input = input.Remove(0, sizeOfChar);

				int a = 0;
				int degree = char_binary.Length - 1;

				foreach (char c in char_binary)
					a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

				output += ((char)a).ToString();
			}

			return output;
		}


		public string Encrypt(string s, string key,ref string newkey)
		{

			s = StringToRightLength(s);
			CutStringIntoBlocks(s);
			key = CorrectKeyWord(key, s.Length / (2 * Blocks.Length));
			newkey = key;
			key = StringToBinaryFormat(key);
			for (int j = 0; j < quantityOfRounds; j++)
			{
				for (int i = 0; i < Blocks.Length; i++)
					Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

				key = KeyToNextRound(key);
			}

			key = KeyToPrevRound(key);
			string result = "";

			for (int i = 0; i < Blocks.Length; i++)
				result += Blocks[i];
			return StringFromBinaryToNormalFormat(result);
		}

		public string Decrypt(string s, string key)
		{
			key = StringToBinaryFormat(key);
			for (int j = 0; j < quantityOfRounds; j++)
			{
				key = KeyToNextRound(key);
			}
			key = KeyToPrevRound(key);
			s = StringToBinaryFormat(s);

			CutBinaryStringIntoBlocks(s);

			for (int j = 0; j < quantityOfRounds; j++)
			{
				for (int i = 0; i < Blocks.Length; i++)
					Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

				key = KeyToPrevRound(key);
			}

			key = KeyToNextRound(key);

			string result = "";

			for (int i = 0; i < Blocks.Length; i++)
				result += Blocks[i];
			result = StringFromBinaryToNormalFormat(result);
			result = result.TrimEnd('#');

			/*StreamWriter sw = new StreamWriter("out2.txt");
			sw.WriteLine(StringFromBinaryToNormalFormat(result));
			sw.Close(); 

			Process.Start("out2.txt");
			*/
			return result;
		}
	}
}
