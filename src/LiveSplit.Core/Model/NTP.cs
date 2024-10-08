﻿using System;
using System.Net;
using System.Net.Sockets;

namespace LiveSplit.Model;

public class NTP
{
    // stackoverflow.com/a/12150289
    public static DateTime Now
    {
        get
        {
            //default Windows time server
            const string NtpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            byte[] ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            IPAddress[] addresses = Dns.GetHostEntry(NtpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            TimeStamp before = TimeStamp.Now;

            socket.Send(ntpData);
            socket.Receive(ntpData);

            TimeStamp after = TimeStamp.Now;
            var delta = TimeSpan.FromTicks((after - before).Ticks / 2);

            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply
            //departed the server for the client, in 64-bit timestamp format."
            const byte ServerReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, ServerReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, ServerReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            ulong milliseconds = (intPart * 1000) + (fractPart * 1000 / 0x100000000L);

            //**UTC** time
            DateTime networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
            DateTime resultingTime = networkDateTime + delta;

            TimeSpan offsetFromLocal = (DateTime.UtcNow - resultingTime).Duration();
            if (offsetFromLocal > TimeSpan.FromHours(1))
            {
                throw new Exception("NTP time is too far off from local time");
            }

            return resultingTime;
        }
    }

    // stackoverflow.com/a/3294698
    private static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}
