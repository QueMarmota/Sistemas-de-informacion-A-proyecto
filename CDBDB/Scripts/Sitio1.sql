CREATE DATABASE Sitio1
use Sitio1
DROP TABLE Proveedor1
select * from Proveedor1
CREATE TABLE Proveedor1
(
	id_Proveedor BIGINT  NOT NULL,
	nombre varchar(100) NOT NULL,	
	email varchar(30) NOT NULL,
	teléfono varchar(30) NOT NULL,	
	CONSTRAINT pk_Proveedor PRIMARY KEY(id_Proveedor)
);
DROP TABLE Sucursal1
INSERT INTO Sucursal1 values('la tienda','direccion','784512',20,'12:30:00','04:30:00')
INSERT INTO Sucursal1 values('la tienda2','direccion2','784512',30,'09:30:00','04:30:00')
CREATE TABLE Sucursal1(
	id_Sucursal BIGINT IDENTITY (1,1) NOT NULL,
	nombre varchar(30) NOT NULL,
	dirección varchar(30) NOT NULL,
	teléfono varchar(30) NOT NULL,
	cantidad_empleados INT NOT NULL,
	hora_apertura TIME NOT NULL,
	hora_cierre TIME NOT NULL,
	CONSTRAINT pk_Sucursal PRIMARY KEY(id_Sucursal)
);
select * FROM Empleado1
DROP TABLE Empleado1
CREATE TABLE Empleado1
(
	RFC BIGINT IDENTITY(1,1) NOT NULL,
	nombre varchar(100) NOT NULL,	
	sueldo real NOT NULL,
	puesto varchar(30) NOT NULL,
	teléfono varchar(30) NOT NULL,	
	dirección varchar(30) NOT NULL,
	fecha_nacimiento date  NOT NULL,
	genero char not null,
	hora_entrada time not null,
	hora_salida time not null,
	id_Sucursal BIGINT NOT NULL,
	CONSTRAINT RFC PRIMARY KEY(RFC),
	CONSTRAINT fk_Sucursal FOREIGN KEY(id_Sucursal)
	REFERENCES Sucursal1(id_Sucursal)
);
delete from producto1
select * from producto1
drop table producto1
CREATE TABLE Producto1(
	id_Producto BIGINT IDENTITY (1,1) NOT NULL,
	nombre varchar(30) NOT NULL,
	precio real NOT NULL,
	caducidad date NOT NULL,
	cantidad INT NOT NULL,
	id_Proveedor BIGINT NOT NULL,
	id_Venta BIGINT,
	CONSTRAINT pk_Producto PRIMARY KEY(id_Producto),
	CONSTRAINT fk_Proveedor FOREIGN KEY(id_Proveedor)
	REFERENCES Proveedor1(id_Proveedor)
);
Drop table Oferta1
CREATE TABLE Oferta1
(
	id_Oferta BIGINT IDENTITY(1,1) NOT NULL,
	descripcion varchar(100) NOT NULL,	
	vigencia date NOT NULL,
	descuento real NOT NULL,
	id_Producto BIGINT NOT NULL,	
	CONSTRAINT pk_Oferta1 PRIMARY KEY(id_Oferta)

);
Drop table Tiene_Of1
CREATE TABLE Tiene_Of1
(
	id_Tiene_Of BIGINT IDENTITY(1,1) NOT NULL,
	id_Oferta BIGINT NOT NULL,	
	id_Producto BIGINT NOT NULL,
	
	CONSTRAINT pk_Tiene_Of PRIMARY KEY(id_Tiene_Of),
		CONSTRAINT pk_Oferta FOREIGN KEY(id_Oferta)
	REFERENCES Oferta1(id_Oferta),
		CONSTRAINT fk_Producto FOREIGN KEY(id_Producto)
	REFERENCES Producto1(id_Producto)
);
DROP TABLE Suministra1
CREATE TABLE Suministra1(

	id_Suministra BIGINT IDENTITY(1,1) NOT NULL,
	id_Sucursal BIGINT NOT NULL,
	id_Proveedor BIGINT NOT NULL,
	CONSTRAINT pk_Suministra PRIMARY KEY(id_Suministra),
	CONSTRAINT fk_ProveedorSuministra FOREIGN KEY(id_Proveedor)
	REFERENCES Proveedor1(id_Proveedor),
	CONSTRAINT fk_SucursalSuministra FOREIGN KEY(id_Sucursal)
	REFERENCES Sucursal1(id_Sucursal)


)

