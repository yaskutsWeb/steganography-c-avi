using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AviHide
{

    public class Engine
    {
        public static string SourceMessageFileName { get; set; }
        public static string SourceVideoFileName { get; set; }
        public static string OutputMessageFileName { get; set; }
        public static string OutputVideoFileName { get; set; }
        public static string Key { get; set; }
        public static double PNSR { get; set; }
        public static int LsbMode { get; set; }
		public static string newkey;
		public static string password = "21";
        // TODO
        public static int BytePerFrame = 1024;


        public static void EncryptAndSave()
        {
			string path123 = "text1.txt"; DES pasha = new DES();
			FileStream stream = new FileStream(SourceMessageFileName, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			stream.Close();
			str=pasha.Encrypt(str, Engine.Key,ref newkey);
			using (StreamWriter sw = File.CreateText(path123))
			{
				sw.WriteLine(str);
			}

			FileStream messageInput = File.OpenRead(path123);
			
            int len = Convert.ToInt32(messageInput.Length);
			
            int it = 0;
            byte[] bytes;

            Video video = new Video(SourceVideoFileName, OutputVideoFileName, len, ".pdf", LsbMode, password);
            video.ResetWriteByte();

           
            
            while ((len - it) > BytePerFrame)
            {
                bytes = new byte[BytePerFrame];
                for (int i = 0; i < BytePerFrame; i++)
                {
                    bytes[i] = (byte) messageInput.ReadByte();
                }

                video.InsertToFrame(bytes);
                it += BytePerFrame;
            }

            bytes = new byte[len - it];
            for (int i = 0; i < len - it; i++)
            {
                bytes[i] = (byte)messageInput.ReadByte();
            }
            video.InsertToFrame(bytes);
            video.CloseWriter();
            messageInput.Close();
			System.IO.File.Delete(path123);

		}


		public static void DecryptAndSave()
        {
            FileStream messageOutput = File.OpenWrite("test2.txt");
            Video video = new Video(SourceVideoFileName, password);

            int len = video.Length;
            byte[] bytes;

            video.ResetReadByte();

            while ((bytes = video.GetByteFromNextFrame()) != null)
            {
                // decrypt bytes
                for (int i = 0; i < bytes.Length; i++)
                {
                    messageOutput.WriteByte(bytes[i]);
                }
            }
			string s = "";
            video.CloseReader();
            messageOutput.Close();		
			StreamReader sr = new StreamReader("test2.txt");

			while (!sr.EndOfStream)
			{
				s += sr.ReadLine();
			}

			sr.Close();
			DES pasha = new DES();
			s=pasha.Decrypt(s, Engine.Key);
			StreamWriter sw = new StreamWriter(OutputMessageFileName);
			sw.WriteLine(s);
			sw.Close();
			System.IO.File.Delete("test2.txt");
			}
	}
}
