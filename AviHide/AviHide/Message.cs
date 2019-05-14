using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace AviHide
{
    class Message
    {
        public String fileName;
        public FileStream stream;

        public Message(String fn)
        {
            OpenFileRead(fn);
        }

        // Returns true if success
        public bool OpenFileRead(String fn)
        {
            fileName = fn;
            try
            {
                stream = File.OpenRead(fn);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        public bool OpenFileWrite(String fn)
        {
            try
            {
                stream = File.OpenWrite(fn);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        public byte[] ReadEncryptedBytes(Int32 offset, Int32 count)
        {
            byte[] originalBytes = new byte[count];

            try
            {
                stream.Read(originalBytes, offset, count);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                return null;
            }

            byte[] encryptedBytes = originalBytes;

            return encryptedBytes;
        }

        public bool WriteDecryptedBytes(byte[] encryptedBytes)
        {
			byte[] decryptedBytes = encryptedBytes;

            try
            {
                stream.Write(decryptedBytes, 0, decryptedBytes.Length);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
