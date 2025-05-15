IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'KeycloakDB')
BEGIN
    CREATE DATABASE KeycloakDB;
END