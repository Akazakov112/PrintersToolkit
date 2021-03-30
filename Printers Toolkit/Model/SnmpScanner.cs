using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Printers_Toolkit.Model.Entity;
using Printers_Toolkit.ViewModel;
using SnmpSharpNet;

namespace Printers_Toolkit.Model
{
    /// <summary>
    /// SNMP сканер сети на наличие принтеров.
    /// </summary>
    class SnmpScanner : MainViewModel, IDisposable
    {
        private readonly string dncpServer;
        private readonly List<Subnet> subnets;
        private readonly CancellationTokenSource cts;
        private readonly CancellationToken token;
        private double progressMax;
        private double progressValue;
        private double progressPercent;
        private bool inProcess;
        private bool stopped;
        private bool disposedValue;

        /// <summary>
        /// Максимальное значения для выполнения.
        /// </summary>
        public double ProgressMax
        {
            get { return progressMax; }
            set { progressMax = value; RaisePropertyChanged(nameof(ProgressMax)); }
        }

        /// <summary>
        /// Значение выполнения.
        /// </summary>
        public double ProgressValue
        {
            get { return progressValue; }
            set { progressValue = value; RaisePropertyChanged(nameof(ProgressValue)); }
        }

        /// <summary>
        /// Процент выполнения.
        /// </summary>
        public double ProgressPercent
        {
            get { return progressPercent; }
            set { progressPercent = value; RaisePropertyChanged(nameof(ProgressPercent)); }
        }

        /// <summary>
        /// Указывает, идет ли процесс сканирования.
        /// </summary>
        public bool InProgress
        {
            get { return inProcess; }
            set { inProcess = value; RaisePropertyChanged(nameof(InProgress)); }
        }

        /// <summary>
        /// Указывает, остановлен ли процесс сканирования.
        /// </summary>
        public bool Stopped
        {
            get { return stopped; }
            set { stopped = value; RaisePropertyChanged(nameof(Stopped)); }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="subnets">Список подсетей для сканирования</param>
        public SnmpScanner(IEnumerable<Subnet> subnets)
        {
            dncpServer = Properties.Settings.Default.DhcpServer;
            this.subnets = subnets.ToList();
            cts = new CancellationTokenSource();
            token = cts.Token;
            InProgress = false;
            Stopped = true;
            ProgressMax = 100;
            ProgressValue = 0;
        }


        /// <summary>
        /// Асинхронный запуск сканирования.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            if (token.IsCancellationRequested)
                return;
            await Task.Run(() => Start());
        }

        /// <summary>
        /// Синхронный запуск сканирования.
        /// </summary>
        public void Start()
        {
            // Статусы.
            InProgress = true;
            Stopped = false;
            // Перебор подсетей и опрос арендованных айпи-адресов по snmp.
            Logger.AddLog("Определение списка IP-адресов подсетей.");
            foreach (var subnet in subnets)
            {
                try
                {
                    subnet.Clients = Dhcp.FindDhcpClients(dncpServer, subnet.Address);
                }
                catch (Exception ex)
                {
                    Logger.AddLog(ex.Message);
                }
            }
            ProgressMax = subnets.Select(x => x.Clients.Count).Sum();
            ProgressValue = 0;
            Logger.AddLog("Опрос IP-адресов.");
            foreach (var subnet in subnets)
            {
                if (subnet.Clients.Any())
                {
                    foreach (var addr in subnet.Clients)
                    {
                        // Токен остановки выполнения.
                        if (token.IsCancellationRequested)
                        {
                            InProgress = false;
                            Stopped = true;
                            return;
                        }
                        // Установка прогресса выполнения.
                        ProgressPercent = ++ProgressValue / ProgressMax;
                        // Переменная ответа.
                        SnmpV1Packet response;
                        // Отправка запроса.
                        try
                        {
                            response = SendRequest(addr.Ip);
                        }
                        catch
                        {
                            continue;
                        }
                        // Запись значений.
                        string netname = response.Pdu.VbList[0].Value.ToString();
                        string model = response.Pdu.VbList[1].Value.ToString();
                        string serialNumber = response.Pdu.VbList[2].Value.ToString();
                        var printer = new Printer(subnet, addr.Ip, model, serialNumber, netname);
                        // Валидация и добавление принтера в репозиторий.
                        List<ValidationResult> results = printer.Validate(new ValidationContext(printer)).ToList();
                        if (!results.Any())
                        {
                            subnet.AddPrinter(printer);
                            DeviceRepository.AddPrinter(printer);
                            Logger.AddLog($"Обнаружен принтер - {printer.Model}, сетевое имя - {printer.NetName}, IP - {printer.Ip}.");
                        }
                    }
                    if (!DeviceRepository.Subnets.Contains(subnet))
                    {
                        DeviceRepository.AddSubnet(subnet);
                    }
                }
                else
                {
                    Logger.AddLog($"В подсети {subnet.Address} нет арендованных IP адресов.");
                }
            }
            Logger.AddLog("Сканирование завершено.");
            InProgress = false;
            Stopped = true;
        }

        /// <summary>
        /// Останавливает сканирование.
        /// </summary>
        public void Stop()
        {
            cts.Cancel();
            Stopped = true;
            Logger.AddLog("Сканирование остановлено.");
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Отправляет SNMP запрос на IP адрес, возвращает ответ.
        /// </summary>
        /// <param name="ipAddress">IP адрес</param>
        /// <returns>Ответ</returns>
        private SnmpV1Packet SendRequest(string ipAddress)
        {
            // Параметры запроса.
            SnmpV1Packet result;
            var community = new OctetString("public");
            var param = new AgentParameters(community)
            {
                Version = SnmpVersion.Ver1
            };
            var agent = new IpAddress(ipAddress);
            var target = new UdpTarget((IPAddress)agent, 161, 450, 1);
            var pdu = new Pdu(PduType.Get);
            // Oid сетевого имени устройства.
            pdu.VbList.Add("1.3.6.1.2.1.1.5.0");
            // Oid модели.
            pdu.VbList.Add("1.3.6.1.2.1.25.3.2.1.3.1");
            // Oid серийного номера.
            pdu.VbList.Add("1.3.6.1.2.1.43.5.1.1.17.1");
            // Отправка запроса.
            result = (SnmpV1Packet)target.Request(pdu, param);
            // Ошибок нет.
            if (result.Pdu.ErrorStatus == 0)
            {
                // Если сетевое имя пустое.
                if (string.IsNullOrWhiteSpace(result.Pdu.VbList[0].Value.ToString()))
                {
                    var kyoceraPdu = new Pdu(PduType.Get);
                    // Oid сетевого имени устройства.
                    kyoceraPdu.VbList.Add("1.3.6.1.4.1.1347.40.10.1.1.5.1");
                    // Oid модели.
                    kyoceraPdu.VbList.Add("1.3.6.1.2.1.25.3.2.1.3.1");
                    // Oid серийного номера.
                    kyoceraPdu.VbList.Add("1.3.6.1.2.1.43.5.1.1.17.1");
                    result = (SnmpV1Packet)target.Request(kyoceraPdu, param);
                }
            }
            // При ошибке запросить с параметрами для зебры.
            else
            {
                var zebraPdu = new Pdu(PduType.Get);
                // Oid сетевого имени устройства.
                zebraPdu.VbList.Add("1.3.6.1.2.1.1.5.0");
                // Oid модели.
                zebraPdu.VbList.Add("1.3.6.1.2.1.25.3.2.1.3.1");
                // Oid серийного номера.
                zebraPdu.VbList.Add("1.3.6.1.4.1.10642.1.9.0");
                result = (SnmpV1Packet)target.Request(zebraPdu, param);
            }
            target.Close();
            return result;
        }
    }
}
