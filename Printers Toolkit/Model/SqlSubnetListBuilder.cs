using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Printers_Toolkit.Model.Entity;

namespace Printers_Toolkit.Model
{
    /// <summary>
    /// Построитель списка подсетей из Sql базы данных.
    /// </summary>
    class SqlSubnetListBuilder
    {
        /// <summary>
        /// Подключение.
        /// </summary>
        private readonly SqlConnection connection;

        /// <summary>
        /// Конструктор со строкой подключения.
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public SqlSubnetListBuilder(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }


        /// <summary>
        /// Возвращает список подсетей асинхронно.
        /// </summary>
        /// <returns>Список подсетей</returns>
        public async Task<List<Subnet>> GetSubnetsAsync()
        {
            return await Task.Run(() => GetSubnets());
        }

        /// <summary>
        /// Возвращает список подсетей синхронно.
        /// </summary>
        /// <returns>Список подсетей</returns>
        private List<Subnet> GetSubnets()
        {
            var subnets = new List<Subnet>();
            // Команда выборки данных.
            string command = $"SELECT Subnet, OSP, City, Address, ShortName " +
                             $"FROM {Properties.Settings.Default.TableName} " +
                             $"WHERE NOT (Subnet IS NULL OR OSP IS NULL OR City IS NULL OR Address IS NULL OR ShortName IS NULL) " +
                             $"AND NOT (OSP = '' OR City = '' OR Address = '' OR ShortName = '' OR ShortName = 'NULL') " +
                             $"Order by OSP";
            // выполнение команды.
            using (SqlCommand com = new SqlCommand(command, connection))
            {
                connection.Open();
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    string address = read.GetString(0);
                    string osp = read.GetString(1);
                    string location = $"{read.GetString(2)} {read.GetString(3)}";
                    string prefix = read.GetString(4);
                    var subnet = new Subnet(address, prefix, osp, location);
                    subnets.Add(subnet);
                }
            }
            return subnets;
        }
    }
}
