using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContextDataManager
{
    public class Credentials
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Credentials(string host, string user, string password)
        {
            Host = host;
            Username = user;
            Password = password;
        }

        public string GetConnectionString(string database_name, int harvest_type_id)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.Host = Host;
            builder.Username = Username;
            builder.Password = Password;
            builder.Database = (harvest_type_id == 3) ? "test" : database_name;
            return builder.ConnectionString;
        }

    }
}
