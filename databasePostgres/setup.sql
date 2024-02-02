create schema sims;
SET search_path TO sims;
create user appuser;

CREATE TABLE incidentType (
    incident_type_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description TEXT
);

CREATE TABLE incident (
    incident_id SERIAL PRIMARY KEY,
    resolved BOOLEAN DEFAULT FALSE,
    reporter VARCHAR(50),
    reported_at TIMESTAMP,
    description TEXT,
    title VARCHAR(100),
    incident_type_id INT REFERENCES incidentType(incident_type_id)
);

GRANT USAGE ON SCHEMA sims TO appuser;
GRANT ALL PRIVILEGES
ON ALL SEQUENCES IN SCHEMA sims TO appuser;

GRANT pg_read_all_data TO appuser;

 insert into sims.incidenttype (name) VALUES 
   ('ticket'), 
   ('issue'),
   ('informational'),
   ('problem');

INSERT INTO sims.incident(
	reporter, reported_at, description, title, incident_type_id)
	VALUES ('Admin', CURRENT_TIMESTAMP, 'The whole internet is down,', 'Internet down', '1');
