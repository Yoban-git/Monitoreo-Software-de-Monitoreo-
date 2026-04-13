

--ver restricciones de una tabla
SELECT 
    tc.CONSTRAINT_NAME,
    tc.CONSTRAINT_TYPE,
    kcu.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE tc.TABLE_NAME = 'Usuarios2';



--base de datos
CREATE DATABASE Monitoreo;
GO

Use Monitoreo;
Go

-- Tabla de Roles
CREATE TABLE Roles (
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    NombreRol NVARCHAR(50) NOT NULL UNIQUE
);
GO
-- Tabla de Permisos
CREATE TABLE Permisos (
    IdPermiso INT PRIMARY KEY IDENTITY(1,1),
    NombrePermiso NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Tabla de Usuarios
CREATE TABLE Usuarios (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Nombre VARCHAR(100),
    UserName VARCHAR(50),
    Email VARCHAR(100),
    Password VARCHAR(255),
    Creado_En DATETIME DEFAULT GETDATE(),
    Modificado_En DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1,
    IdRol INT,
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

-- Trigger para actualizar la fecha de modificación
CREATE TRIGGER trg_UpdateUsuarios
ON Usuarios
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE u
    SET Modificado_En = GETDATE()
    FROM Usuarios u
    INNER JOIN Inserted i ON u.Id = i.Id;
END;
GO
select * from Usuarios;

-- Relación Rol-Permiso (muchos a muchos)
CREATE TABLE RolPermiso (
    IdRol INT NOT NULL,
    IdPermiso INT NOT NULL,
    PRIMARY KEY (IdRol, IdPermiso),
	FOREIGN KEY (IdRol) REFERENCES Roles(IdRol) ON DELETE CASCADE ON UPDATE CASCADE, 
	FOREIGN KEY (IdPermiso) REFERENCES Permisos(IdPermiso) ON DELETE CASCADE ON UPDATE CASCADE
);
GO
select * from RolPermiso;

--Vista para RolPermisos
CREATE VIEW VistaRolesPermisos
AS
SELECT
	r.IdRol,
	p.IdPermiso,
	r.NombreRol AS Rol,
	p.NombrePermiso AS Permiso
FROM RolPermiso rp
INNER JOIN Roles r ON rp.IdRol = r.IdRol
INNER JOIN Permisos p ON rp.IdPermiso = p.IdPermiso;
GO

-- Tabla Sensores
CREATE TABLE Sensores (
    IdSensor INT PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(100),
    Tag VARCHAR(100),
	ValorMinimo FLOAT DEFAULT 1,
	ValorMaximo FLOAT DEFAULT 1,
    Creado_En DATETIME2(0) DEFAULT GETDATE(),
    Modificado_En DATETIME2(0) DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);
GO
SELECT * FROM Sensores;

-- Trigger para actualizar la fecha de modificación
CREATE TRIGGER trg_Update_Sensores
ON Sensores
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
    SET Modificado_En = GETDATE()
    FROM Sensores s
    INNER JOIN Inserted i ON s.IdSensor = i.IdSensor;
END;
GO

-- Tabla Datos_Sensores 
CREATE TABLE DatosSensores ( 
	IdDato INT PRIMARY KEY IDENTITY(1,1),
	IdSensor INT,
	Dato FLOAT,
	Creado_En DATETIME2(0) DEFAULT GETDATE(),
	CONSTRAINT FK_Datos_Sensores_Sensores FOREIGN KEY (IdSensor) REFERENCES Sensores(IdSensor) 
        ON UPDATE CASCADE ON DELETE CASCADE
);

SELECT * FROM DatosSensores
--Tabla para los datos de un excel
CREATE TABLE DatosExcel(
	IdDatoE INT PRIMARY KEY IDENTITY(1,1),
	TagSensor VARCHAR(100),
	Dato FLOAT,
	Creado_En DATETIME2(0) DEFAULT GETDATE()
);
select * from DatosExcel;
