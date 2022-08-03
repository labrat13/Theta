using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Class contains some utility functions
    /// </summary>
    public static class MUtility
    {
        /// <summary>
        /// NR-Convert bytes to hex string
        /// </summary>
        /// <param name="byteArray">Массив байт для конверсии в Hex строку.</param>
        /// <returns>Возвращает Hex-строку.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public static string BytesToHex(byte[] byteArray)
        {
            //throw new NotImplementedException();
            StringBuilder sb = new StringBuilder(byteArray.Length * 2);
            for (int i = 0; i < byteArray.Length; i++)
            {
                Int32 b = (Int32)byteArray[i];
                sb.Append(getHexChar(b >> 4));
                sb.Append(getHexChar(b & 15));
            }
            return sb.ToString();
        }
        /// <summary>
        /// NR-Convert hex string to bytes
        /// </summary>
        /// <param name="hexStr">Строка Hex для конверсии в байты.</param>
        /// <returns>Возвращает массив байт.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public static byte[] HexToBytes(string hexStr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return char equivalent of specified number in range 0..15
        /// </summary>
        /// <param name="val">Number in range 0..15</param>
        /// <returns>Returns hex char</returns>
        private static Char getHexChar(int val)
        {
            switch (val)
            {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'C';
                case 13:
                    return 'D';
                case 14:
                    return 'E';
                case 15:
                    return 'F';
                default:
                    throw new Exception("Invalid hex number");
            }
        }

        /// <summary>
        /// NFT-Check that char is hex digit 0..9,A..H
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        ///<exception cref=""></exception>
        public static bool isHexDigit(char c)
        {
            if (Char.IsDigit(c)) return true;
            char t = Char.ToUpper(c);
            if ((t == 'A') || (t == 'B') || (t == 'C') || (t == 'D') || (t == 'E') || (t == 'F')) return true;
            return false;
        }
        /// <summary>
        /// NFT-Check that char is hex digit 0..9,A..H
        /// </summary>
        /// <param name="t">Char for check</param>
        /// <returns>Returns true if char is hex char, False otherwise.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public static bool isHexDigit2(char t)
        {
            //TODO: выяснить, какая из них быстрее, ту и оставить
            if ((t == '1') || (t == '2') || (t == '3') || (t == '4') || (t == '5')) return true;
            if ((t == '6') || (t == '7') || (t == '8') || (t == '9') || (t == '0')) return true;
            if ((t == 'A') || (t == 'B') || (t == 'C') || (t == 'D') || (t == 'E') || (t == 'F')) return true;
            if ((t == 'a') || (t == 'b') || (t == 'c') || (t == 'd') || (t == 'e') || (t == 'f')) return true;
            return false;
        }
        /// <summary>
        /// Remove chars that non-safe to XML parsing
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string createXmlSafeString(string s)
        {
            Char rep = '?';
            string t = s.Replace('<', rep);
            string tt = t.Replace('>', rep);
            string ttt = tt.Replace('&', rep);
            return ttt.Replace('"', rep);
        }

        /// <summary>
        /// Remove chars that non-safe to XML parsing
        /// </summary>
        /// <param name="s">String contans non-safe XML symbols</param>
        /// <returns>Returns XML-safe string</returns>
        /// <remarks>
        /// Unsafe symbols replaced by ?. 
        /// Однако по тестам в XML-файлах при сериализации такие символы заменяются на HTML-аналоги.
        /// То есть, надобности в функции вроде бы нет.
        /// </remarks>
        /// <seealso cref=""/>
        public static string createXmlSafeString2(string s)
        {
            //TODO: выяснить, какая из них быстрее, ту и оставить
            Char rep = '?';
            StringBuilder sb = new StringBuilder(s);
            sb.Replace('<', rep);
            sb.Replace('>', rep);
            sb.Replace('&', rep);
            sb.Replace('"', rep);
            return sb.ToString();
        }

        /// <summary>
        /// Проверить и укоротить введенное пользователем имя
        /// </summary>
        /// <param name="s">Имя или текст, введенный пользователем</param>
        /// <param name="len">Максимальная разрешенная длина строки</param>
        /// <returns>Возвращает обработанную строку.</returns>
        public static String processString(String s, int len)
        {
            string res;
            //если нуль, то пусть лучше будет пустая строка
            if (String.IsNullOrEmpty(s))
                return String.Empty;
            //remove white spaces
            res = s.Trim();
            //cut to len chars
            if (res.Length > len)
                res = res.Substring(0, len);
            return res;
        }


    }
}
