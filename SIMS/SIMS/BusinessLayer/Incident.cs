﻿using Npgsql;

namespace SIMS
{
    public class Incident : DBBase
    {
        public int Incident_id { get; set; }
        public bool Resolved { get; set; }
        public string Reporter { get; set; } = "";
        public DateTime Reported_at { get; set; } = DateTime.Now;
        public string Description { get; set; } = "";
        public string Title { get; set; } = "";
        public int Incident_type { get; set; } = 1;
        public string Instance_Location { get; set; } = Environment.GetEnvironmentVariable("default_instlocation") ?? "Specify. Note default unset";
        public string Instance_Name { get; set; } = "No Instance";

        public Incident() { }

        public Incident(int id)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from sims.incident where Incident_id = {id}", db))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            Incident_id = Convert.ToInt32(reader["Incident_id"]);
                            Reporter = (string)reader["reporter"];
                            Description = (string)reader["Description"];
                            Title = (string)reader["Title"];
                            Reported_at = Convert.ToDateTime(reader["Reported_at"]);
                            Incident_type = Convert.ToInt32(reader["Incident_type_id"]);
                            Instance_Location = (string)reader["instance_location"];
                            Instance_Name = (string)reader["instance_name"];
                        }
                    }
                }
                db.Close();
            }
        }

        public List<Incident> GetList()
        {
            List<Incident> result = new List<Incident>();
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from sims.incident where resolved = false order by Reported_at desc", db))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Incident item = new Incident()
                            {
                                Incident_id = Convert.ToInt32(reader["Incident_id"]),
                                Reporter = (string)reader["reporter"],
                                Description = (string)reader["Description"],
                                Title = (string)reader["Title"],
                                Reported_at = Convert.ToDateTime(reader["Reported_at"]),
                                Incident_type = Convert.ToInt32(reader["Incident_type_id"]),
                                Instance_Location = (string)reader["instance_location"],
                                Instance_Name = (string)reader["instance_name"]
                            };
                            result.Add(item);
                        }
                    }
                }
                db.Close();
            }
            return result;
        }

        public void Save()
        {
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                string sql = "";
                if (Incident_id == 0)
                {
                    sql += $"INSERT INTO sims.incident(resolved, reporter, reported_at, description, title, incident_type_id, instance_location, instance_name) ";
                    sql += $"VALUES (@resolved, @reporter, @reported_at, @description, @title, @incident_type_id, @instance_location, @instance_name);";
                }
                else
                {
                    sql += $"update sims.incident set resolved = @resolved, reporter = @reporter, reported_at = @reported_at, instance_location = @instance_location, instance_name = @instance_name,";
                    sql += $"description = @description, title = @title, incident_type_id = @incident_type_id where Incident_id = {Incident_id};";
                }
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, db))
                {
                    cmd.Parameters.AddWithValue("reporter", Reporter);
                    cmd.Parameters.AddWithValue("reported_at", Reported_at);
                    cmd.Parameters.AddWithValue("description", Description);
                    cmd.Parameters.AddWithValue("title", Title);
                    cmd.Parameters.AddWithValue("resolved", Resolved);
                    cmd.Parameters.AddWithValue("incident_type_id", Incident_type);
                    cmd.Parameters.AddWithValue("instance_name", Instance_Name);
                    cmd.Parameters.AddWithValue("instance_location", Instance_Location);
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }
    }
}
