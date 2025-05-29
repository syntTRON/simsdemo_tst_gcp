
using Npgsql;

namespace SIMSAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            TestInit();

            app.Run();
        }
        private static void TestInit()
        {
            string TestSql = "SELECT EXISTS (SELECT FROM information_schema.tables t WHERE table_schema = 'sims' and table_name = 'simsuser');";
            bool NeedCreation = false;

            string connectionString = Environment.GetEnvironmentVariable("postgresdb") ?? "Host=localhost;Username=postgresadmin;Password=1234;Database=db1";
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(TestSql, db))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
			try
			{
                            bool nc = false;
                            if(reader.HasRows == true){reader.Read();}
                            nc = reader.GetBoolean(0);
                            if(nc == false) {NeedCreation = true;}
			}
			catch(Exception e)
			{
                            Console.WriteLine("Exeption trown in Database check");
			    NeedCreation = true;
			}
                    }
                }
                if (NeedCreation == true)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, db))
                    {
                        Console.WriteLine("Database was not initalized and shuld have been created by now But will now be initalized");
                        cmd.ExecuteNonQuery();
                    }
                }
                db.Close();
            }


        }
        private const string SQL = "create schema sims;\nSET search_path TO sims;\ncreate user appuser;\n\nCREATE TABLE incidentType (\n    incident_type_id SERIAL PRIMARY KEY,\n    name VARCHAR(50) NOT NULL,\n    description TEXT\n);\n\nCREATE TABLE incident (\n    incident_id SERIAL PRIMARY KEY,\n    resolved BOOLEAN DEFAULT FALSE,\n    reporter VARCHAR(50),\n    reported_at TIMESTAMP,\n    description TEXT,\n    title VARCHAR(100),\n    incident_type_id INT REFERENCES incidentType(incident_type_id)\n);\n\nCREATE TABLE simsuser (\n    user_id SERIAL PRIMARY KEY,\n    IsActive BOOLEAN DEFAULT FALSE,\n    IsAdmin BOOLEAN DEFAULT FALSE,\n    LastLogin TIMESTAMP,\n    Username VARCHAR(50),\n    PWDHash VARCHAR(200)\n);\n\nGRANT USAGE ON SCHEMA sims TO appuser;\nGRANT ALL PRIVILEGES\nON ALL SEQUENCES IN SCHEMA sims TO appuser;\n\nGRANT pg_read_all_data TO appuser;\n\nINSERT INTO sims.incidenttype (name) VALUES\n   ('ticket'),\n   ('issue'),\n   ('informational'),\n   ('problem');\n\nINSERT INTO sims.incident(\n\treporter, reported_at, description, title, incident_type_id)\n\tVALUES ('Admin', CURRENT_TIMESTAMP, 'The whole internet is down,', 'Internet down', '1');\n\nINSERT INTO sims.simsuser(\n\tIsAdmin, IsActive, Username, PWDHash, LastLogin)\n\tVALUES (true, true, 'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', CURRENT_TIMESTAMP);\n\nINSERT INTO sims.simsuser(\n\tIsAdmin, IsActive, Username, PWDHash, LastLogin)\n\tVALUES (false, true, 'user', '04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb', CURRENT_TIMESTAMP);\n";
    }
}
