using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Net.Mail;

namespace Security
{
    /// <summary>
    /// 
    /// </summary>
    public class secClass
    {
        private string plainText;
        private string hashText;

        ///<summary>Bad char is in the Text.</summary>
        public char _rChar = ' ';

        ///<summary>
        ///Constructive function for Controller class.
        ///</summary>
        public secClass(string arg)
        {
            plainText = arg;
        }

        ///<summary>
        ///<para>This function checks length of plainText.</para>
        ///If the length is equal or bigger than 8, result will be true.
        ///</summary>
        public Boolean lengthChecker()
        {
            if (this.plainText.Length < 8)
            {
                return true;
            }
            return false;
        }

        ///<summary>
        ///This function check alphanumeric char.
        ///</summary>
        public Boolean badCharChecker()
        {
            List<int> turkishASCII = new List<int> { 220, 252, 286, 287, 304, 305, 350, 351 };
            foreach (char item in this.plainText)
            {
                if ( (47 < (int)item && (int)item < 58) || (64 < (int)item && (int)item < 91) || (96 < (int)item && (int)item < 123) 
                    || turkishASCII.Contains((int)item))
                {
                    continue;
                }
                else
                {
                    _rChar = item;
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        ///Generate MD5 Password.
        ///</summary>
        public string md5Generator()
        {
            byte[] result;
            MD5 hashClass = new MD5CryptoServiceProvider();
            hashClass.ComputeHash(ASCIIEncoding.ASCII.GetBytes(plainText));
            result = hashClass.Hash;
            foreach (byte b in result)
            {
                hashText += b.ToString("x2");
            }
            return hashText;
        }

        ///<summary>
        ///First Item -> Bad Char, Second Item -> Length, Third Item -> Lower Char,
        ///Fourth Item -> Upper Char, Sixth Item -> Digit
        ///</summary>
        public bool[] isPassword()
        {
            bool[] res = new bool[] {false, false, false, false, false };
            if (this.badCharChecker())
                res[0] = true;
            if (this.lengthChecker())
                res[1] = true;
            if (!this.plainText.Any(char.IsUpper))
                res[2] = true;
            if (!this.plainText.Any(char.IsLower))
                res[3] = true;
            if (!this.plainText.Any(char.IsDigit))
                res[4] = true;
            return res;
        }

        ///<summary>
        ///First Item -> Bad Char, Second Item -> Length, 
        ///Third Item -> is first char digit 
        ///</summary>
        public bool[] isUsername()
        {
            bool[] res = new bool[] { false, false, false };
            if (this.badCharChecker())
                res[0] = true;
            if (this.plainText.Length < 3)
                res[1] = true;
            if (this.plainText.Length != 0)
                if(char.IsDigit(this.plainText[0]))
                    res[2] = true;
            return res;
        }

        /// <summary>
        /// This function checks that is string MD5 or not.
        /// </summary>
        /// <returns>True or False</returns>
        public bool isMD5()
        {
            List<int> arrange = Enumerable.Range(48, 10).ToList<int>();
            arrange.AddRange(Enumerable.Range(97, 6));
            if (this.plainText.Length != 32)
                return false;
            foreach (char item in this.plainText)
                if (!arrange.Contains((int)item))
                    return false;
            return true;
        }
        /// <summary>
        /// Entered string is mail or not.
        /// </summary>
        /// <returns>True or False</returns>
        public bool isEmail()
        {
            try
            {
                
                MailAddress mail = new MailAddress(this.plainText);
            }
            catch(System.FormatException)
            {
                return false;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
            return true;
        }

        private string cesar(int _id, string _username)
        {
            string _alphabet = "abcdefghijklmnoprstuvyzqwx";
            string _numbers = "0123456789";
            string _hashText = string.Empty;
            int _shift = 0;
            for (int i = 0; i < _username.Length; i++)
            {
                if (_alphabet.Contains(_username[i]))
                {
                    _shift = (_alphabet.IndexOf(_username[i]) + _id) % 26;
                    _hashText += _alphabet[_shift];
                }
                else if (_numbers.Contains(_username[i]))
                {
                    _shift = (_numbers.IndexOf(_username[i]) + _id) % 10;
                    _hashText += _numbers[_shift];
                }
            }
            return _hashText;
        }

        /// <summary>
        /// Generate key for reset password.
        /// </summary>
        /// <param name="_id">User ID</param>
        /// <returns>Key as string</returns>
        public string key_generator(int _id)
        {
            string _shiftedText = cesar(_id, plainText);
            string _md5Result = new secClass(_shiftedText).md5Generator();
            if (_id % 4 == 0) //0
                return _md5Result.Substring(0,8);
            else if (_id % 4 == 1) //1 
                return _md5Result.Substring(8, 8);
            else if(_id % 4 == 2) //2
                return _md5Result.Substring(16, 8);
            else //3
                return _md5Result.Substring(24, 8);
        }
    }
}
