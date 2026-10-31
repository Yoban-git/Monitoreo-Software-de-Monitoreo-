-- Insertar Roles
INSERT INTO Roles (NombreRol) VALUES ('SuperUsuario');
INSERT INTO Roles (NombreRol) VALUES ('Administrador');
INSERT INTO Roles (NombreRol) VALUES ('Usuario');
INSERT INTO Roles (NombreRol) VALUES ('Invitado');
select * from Roles
-- Insertar Permisos
INSERT INTO Permisos (NombrePermiso) VALUES ('Activar_Cuentas');
INSERT INTO Permisos (NombrePermiso) VALUES ('Activar_Sensor');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_DatoSensor');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_Excel');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_Permisos');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_Roles');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_Sensores');
INSERT INTO Permisos (NombrePermiso) VALUES ('Agregar_Usuarios');
INSERT INTO Permisos (NombrePermiso) VALUES ('Cargar_Excel');
INSERT INTO Permisos (NombrePermiso) VALUES ('Editar_DatoSensor');
INSERT INTO Permisos (NombrePermiso) VALUES ('Editar_Permisos');
INSERT INTO Permisos (NombrePermiso) VALUES ('Editar_Roles');
INSERT INTO Permisos (NombrePermiso) VALUES ('Editar_Usuarios');
INSERT INTO Permisos (NombrePermiso) VALUES ('Eliminar_Permisos');
INSERT INTO Permisos (NombrePermiso) VALUES ('Eliminar_Roles');
INSERT INTO Permisos (NombrePermiso) VALUES ('Gestionar_Cuentas');
INSERT INTO Permisos (NombrePermiso) VALUES ('Gestionar_Sensores');
INSERT INTO Permisos (NombrePermiso) VALUES ('Monitorear_Sensores');
INSERT INTO Permisos (NombrePermiso) VALUES ('Ver_Sensores');
INSERT INTO Permisos (NombrePermiso) VALUES ('Ver_Sensores_Activos');
INSERT INTO Permisos (NombrePermiso) VALUES ('Ver_Usuarios');
INSERT INTO Permisos (NombrePermiso) VALUES ('Ver_Usuarios_Activos');
select * from Permisos;
-- Asignar Permisos a Roles
-- SuperUsuario -> todos los permisos
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,1);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,2);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,3);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,4);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,5);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,6);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,7);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,8);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,9);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,10);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,11);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,12);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,13);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,14);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,15);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,16);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,17);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,18);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,19);
INSERT INTO RolPermiso (IdRol, IdPermiso)
values(1,21);
select * from RolPermiso

--Insertar SUPER USUARIO u:Super p:Super
insert into Usuarios (Nombre,UserName,Email,Password,Activo,IdRol)
values('Super_User','Super','Super@sol.com','$2a$12$mlteOP64uLfGrI4S1.N0q.aCJyDv34I32ks31xY31wcFBPdTSSw1W','True',1);

select * from Sensores;
