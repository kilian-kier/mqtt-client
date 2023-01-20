using System;
using System.Collections.Generic;

namespace MQTT_GUI.MQTT.models
{
    public class MQTT
    {
        public byte ControlHeader;
        public byte[] RemainingLength;
        public byte[] Header;
        public byte[] Payload;

        public byte[] GetBytes()
        {
            var ret = new List<byte> {ControlHeader};

            ret.AddRange(RemainingLength);
            ret.AddRange(Header);
            ret.AddRange(Payload);
            
            return ret.ToArray();
        }

        public void FromBytes(byte[] data)
        {
            if (data.Length == 0)
                return;
            ControlHeader = data[0];
            const byte continuation = 0b10000000;
            var remainingLength = 1;
            for (var i = 2; i < 5; i++)
            {
                var check = data[i] & continuation;
                if (check != 0)
                {
                    remainingLength++;
                }
                else
                {
                    break;
                }
            }

            RemainingLength = new byte[remainingLength];
            for (var i = 0; i < remainingLength; i++)
            {
                RemainingLength[i] = data[1 + i];
            }

            Header = new byte[data.Length - remainingLength - 1];
            
            for (var i = remainingLength + 1; i < data.Length; i++)
            {
                Header[i - remainingLength - 1] = data[i];
            }

            Payload = Array.Empty<byte>();
        }

        public int FromSubscribedBytes(byte[] data)
        {
            if (data.Length == 0)
                return 0;
            ControlHeader = data[0];
            const byte continuation = 0b10000000;
            var remainingByteLength = 1;
            for (var i = 2; i < 5; i++)
            {
                var check = data[i] & continuation;
                if (check != 0)
                {
                    remainingByteLength++;
                }
                else
                {
                    break;
                }
            }

            RemainingLength = new byte[remainingByteLength];
            for (var i = 0; i < remainingByteLength; i++)
            {
                RemainingLength[i] = data[1 + i];
            }

            var remainingLength = 0;
            if (remainingByteLength == 1)
            {
                remainingLength = RemainingLength[0];
            }
            else
            {
                var bytes = new byte[remainingLength];
                for (var i = 0; i < remainingByteLength; i++)
                {
                    bytes[i] = RemainingLength[remainingLength - 1 - i];
                }

                remainingLength = BitConverter.ToInt32(bytes, 0);
            }

            var headerLength = BitConverter.ToInt16(new[] {data[remainingByteLength + 2], data[remainingByteLength + 1]}, 0);
            Header = new byte[headerLength + 1];
            Header[0] = data[remainingByteLength + 1];
            for (var i = remainingByteLength + 2; i < remainingByteLength + 2 + headerLength + 1; i++)
            {
                Header[i - remainingByteLength - 2] = data[i];
            }

            var payloadLength = remainingLength - headerLength - 2;
            Payload = new byte[payloadLength];
            var idx = remainingByteLength + headerLength + 3;
            for (var i = idx; i < idx + payloadLength; i++)
            {
                Payload[i - remainingByteLength - headerLength - 3] = data[i];
            }

            return idx + payloadLength;
        }
    }
}