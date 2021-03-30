using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Printers_Toolkit.Model.Entity
{
    /// <summary>
    /// DHCP клиент.
    /// </summary>
    public class DhcpClient
    {
        /// <summary>
        /// Ip адрес.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="ip">Ip адрес</param>
        public DhcpClient(string ip)
        {
            Ip = ip;
        }
    }

    /// <summary>
    /// Класс статических методов работы с DHCP.
    /// </summary>
    class Dhcp
    {
        /// <summary>
        /// Представляет информацию по клиенту DHCP.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct DHCP_CLIENT_INFO_ARRAY
        {
            public uint NumElements;
            public IntPtr Clients;
        }

        /// <summary>
        /// Представляет UID по клиенту DHCP.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct DHCP_CLIENT_UID
        {
            public uint DataLength;
            public IntPtr Data;
        }

        /// <summary>
        /// Представляет информацию по клиенту DHCP.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DHCP_CLIENT_INFO
        {
            public uint ip;
            public uint subnet;

            private DHCP_CLIENT_UID mac;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientComment;
        }

        /// <summary>
        /// Возвращает uint представление Ip адреса.
        /// </summary>
        /// <param name="ip">Ip адрес в строковом представлении</param>
        /// <returns></returns>
        private static uint StringIPAddressToUInt32(string ip)
        {
            // Побитовоая конвертация ip адреса.
            IPAddress i = IPAddress.Parse(ip);
            byte[] ipByteArray = i.GetAddressBytes();
            uint ipUint = (uint)ipByteArray[0] << 24;
            ipUint += (uint)ipByteArray[1] << 16;
            ipUint += (uint)ipByteArray[2] << 8;
            ipUint += ipByteArray[3];
            return ipUint;
        }

        /// <summary>
        /// Возвращает строковое представление Ip адреса.
        /// </summary>
        /// <param name="ip">Ip адрес в uint</param>
        /// <returns></returns>
        private static string UInt32IPAddressToString(uint ip)
        {
            IPAddress i = new IPAddress(ip);
            string[] ipArray = i.ToString().Split('.');
            return ipArray[3] + "." + ipArray[2] + "." + ipArray[1] + "." + ipArray[0];
        }

        /// <summary>
        /// Возвращает перечисление клиентов DHCP.
        /// </summary>
        /// <param name="ServerIpAddress"></param>
        /// <param name="SubnetAddress"></param>
        /// <param name="ResumeHandle"></param>
        /// <param name="PreferredMaximum"></param>
        /// <param name="ClientInfo"></param>
        /// <param name="ElementsRead"></param>
        /// <param name="ElementsTotal"></param>
        /// <returns></returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint DhcpEnumSubnetClients(
            string ServerIpAddress, uint SubnetAddress, ref uint ResumeHandle,
            uint PreferredMaximum, out IntPtr ClientInfo, ref uint ElementsRead, ref uint ElementsTotal
        );

        /// <summary>
        /// Возвращает список клиентов подсети на сервере DHCP.
        /// </summary>
        /// <param name="server">Сервер</param>
        /// <param name="subnet">Подсеть</param>
        /// <returns>Список клиентов</returns>
        public static List<DhcpClient> FindDhcpClients(string server, string subnet)
        {
            List<DhcpClient> foundClients = new List<DhcpClient>();
            uint parsedMask = StringIPAddressToUInt32(subnet);
            uint resumeHandle = 0;
            uint numClientsRead = 0;
            uint totalClients = 0;
            _ = DhcpEnumSubnetClients(server, parsedMask, ref resumeHandle, 65536, out IntPtr info_array_ptr, ref numClientsRead, ref totalClients);
            var rawClients = (DHCP_CLIENT_INFO_ARRAY)Marshal.PtrToStructure(info_array_ptr, typeof(DHCP_CLIENT_INFO_ARRAY));
            IntPtr current = rawClients.Clients;
            for (int i = 0; i < (int)rawClients.NumElements; i++)
            {
                DHCP_CLIENT_INFO rawMachine = (DHCP_CLIENT_INFO)Marshal.PtrToStructure(Marshal.ReadIntPtr(current), typeof(DHCP_CLIENT_INFO));
                string ip = UInt32IPAddressToString(rawMachine.ip);
                DhcpClient thisClient = new DhcpClient(ip);
                foundClients.Add(thisClient);
                current = (IntPtr)((int)current + Marshal.SizeOf(typeof(IntPtr)));
            }
            return foundClients;
        }
    }
}
