using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SerialDebug
{
    public class CSendParam
    {
        private SendParamFormat _Format;
        private SendParamMode _Mode;
        private int _DelayTime;
        private readonly string _Data;
        private readonly byte[] _DataBytes = null;

        public CSendParam(SendParamFormat format, SendParamMode mode, int delayTime, byte[] data, int startIndex, int count)
        {
            _Format = format;
            _Mode = mode;
            _DelayTime = delayTime;
            _Data = string.Empty;
            if (data != null)
            {
                _DataBytes = new byte[count];
                Array.Copy(data, startIndex, _DataBytes, 0, count);

                if (Format == SendParamFormat.Hex)
                {
                    _Data = BitConverter.ToString(_DataBytes).Replace('-', ' ').TrimEnd(new char[] { ' ' });
                }
                else
                {
                    _Data = System.Text.ASCIIEncoding.Default.GetString(_DataBytes);
                }
            }
        }

        public CSendParam(SendParamFormat format, SendParamMode mode, int delayTime, string data)
        {
            _Format = format;
            _Mode = mode;
            _DelayTime = delayTime;
            _Data = data;

            switch (_Format)
            {
                case SendParamFormat.ASCII:
                    _DataBytes = System.Text.ASCIIEncoding.Default.GetBytes(_Data);
                    break;
                case SendParamFormat.Hex:

                    string inputText = Regex.Replace(_Data, @"[0-9A-Fa-f]{2}", "$0 ");
                    string[] strArray = inputText.Split(new string[] { ",", " ", "0x", ",0X", "��", "(", ")","\r","\n" }, StringSplitOptions.RemoveEmptyEntries);


                    StringBuilder sbOut = new StringBuilder();
                    foreach (string s in strArray)
                    {
                        sbOut.AppendFormat("{0:X2} ", Convert.ToByte(s, 16));
                    }
                    _Data = sbOut.ToString().TrimEnd(' ');


                    _DataBytes = Array.ConvertAll<string, byte>(strArray, new Converter<string, byte>(HexStringToByte));

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ʮ�������ַ���תʮ���ơ�
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        byte HexStringToByte(string hexStr)
        {
            return Convert.ToByte(hexStr, 16);
        }

        public SendParamFormat Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        public SendParamMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        public int DelayTime
        {
            get { return _DelayTime; }
            set { _DelayTime = value; }
        }

        public int DataLen
        {
            get
            {
                if (_DataBytes != null)
                {
                    return _DataBytes.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Data
        {
            get { return _Data; }
        }

        public string HexString
        {
            get
            {
                return string.Format("{0} ", BitConverter.ToString(_DataBytes).Replace('-', ' '));
            }
        }

        public string ASCIIString
        {
            get { return System.Text.ASCIIEncoding.Default.GetString(_DataBytes); }
        }

        public string DecString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (byte b in _DataBytes)
                {
                    sb.AppendFormat("{0} ", Convert.ToInt32(b));
                }

                return sb.ToString();
            }

        }

        public byte[] DataBytes
        {
            get
            {
                return _DataBytes;
            }
        }


        public string ParameterString
        {
            get
            {
                return string.Format("{0}:{1}:{2}", (int)_Format, (int)_Mode, _DelayTime);
            }
        }
    }


    public enum SendParamFormat : int
    {
        ASCII = 0,
        Hex = 1
    }

    public enum SendParamMode : int
    {
        //SendDelayTime = 0,// ��������ʱ����
        SendAfterLastSend = 0,//��֡������ɺ�
        SendAfterReceived = 1//���յ�����֡��
    }
}
