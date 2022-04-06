using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace KeyboardLed
{
    internal class KeyboardController
    {
        private const string TestSequence = "5";
        private const string ResultSquence = "ok!";

        private const string UpdateSquence = "6";

        private Color[][] _pixelsBuffer = new Color[][] 
        {
            new Color[16],
            new Color[17],
            new Color[17],
            new Color[13],
            new Color[13],
            new Color[11]
        };

        private SerialPort _port;

        public KeyboardController()
        {
            var ports = SerialPort.GetPortNames();

            for(int i = 2; i < ports.Length; i++)
            {
                try
                {
                    var port = new SerialPort(ports[i], 115200);
                    port.RtsEnable = true;
                    port.DtrEnable = true;
                    port.ReadTimeout = 2000;
                    port.NewLine = "\n";
                    port.Open();
                    port.Write(TestSequence);
                    var result = port.ReadLine();
                    if(result == ResultSquence)
                    {
                        _port = port;
                        break;
                    }
                    port.Close();
                }
                catch(Exception)
                {

                }
            }

            if(_port == null)
            {
                throw new Exception("Port not found");
            }

        }

        public void SetPixelColor(int line, int pixel, Color color)
        {
            if(line >= _pixelsBuffer.Length || line < 0)
            {
                return;
            }

            if (pixel >= _pixelsBuffer[line].Length || pixel < 0)
            {
                return;
            }

            _pixelsBuffer[line][pixel] = color;
        }

        public void SetLineColor(int line, Color color)
        {
            if (line >= _pixelsBuffer.Length || line < 0)
            {
                return;
            }

            for(int i = 0; i < _pixelsBuffer[line].Length; i++)
            {
                _pixelsBuffer[line][i] = color;
            }
        }

        public void Fill(Color color)
        {
            for (int i = 0; i < _pixelsBuffer.Length; i++)
            {
                for(int j = 0; j < _pixelsBuffer[i].Length; j++)
                {
                    _pixelsBuffer[i][j] = color;
                }
            }
        }

        public void PushLineColor(int line, Color color)
        {
            var buffer = new byte[] {(byte)'0', (byte)line, color.Red, color.Green, color.Blue };
            WriteToPort(buffer);
        }

        public void PushBuffer()
        {
            var buffer = new List<byte>();
            buffer.Add((byte)'1');

            var i = 0;
            var j = 0;
            var counter = 0;

            while(true)
            {
                if(j == _pixelsBuffer[i].Length)
                {
                    i++;
                    j = 0;
                }

                if (_pixelsBuffer[i].Length > j)
                {
                    buffer.AddRange(new byte[] { (byte)i, (byte)j, _pixelsBuffer[i][j].Red, _pixelsBuffer[i][j].Green, _pixelsBuffer[i][j].Blue });
                }

                if((counter % 8) == 0 && counter != 0)
                {
                    var arrayBuffer = buffer.ToArray();
                    WriteToPort(arrayBuffer);
                    buffer.RemoveRange(1, buffer.Count - 1);
                    Thread.Sleep(1);
                }

                if(i == _pixelsBuffer.Length - 1 && j == _pixelsBuffer[i].Length - 1)
                {
                    var arrayBuffer = buffer.ToArray();
                    WriteToPort(arrayBuffer);
                    break;
                }


                j++;
                counter++;
            }

            Thread.Sleep(1);

            _port.Write(UpdateSquence);
        }

        public void PushFillColor(Color color)
        {
            WriteToPort((byte)'2', color.Red, color.Green, color.Blue);
        }

        public void PushClear()
        {
            WriteToPort((byte)'3');
        }

        private void WriteToPort(params byte[] bytes)
        {
            if(!_port.IsOpen)
            {
                try
                {
                    _port.Open();
                }
                catch(FileNotFoundException)
                {
                    MessageBox.Show("Keyboard disconnected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }

            try
            {
                _port.Write(bytes, 0, bytes.Length);
            }
            catch(InvalidOperationException)
            {
                MessageBox.Show("Keyboard disconnected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }

    struct Color
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

}
