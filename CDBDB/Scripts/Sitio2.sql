select * from proveedor2
DROP TABLE proveedor2
CREATE TABLE Proveedor2
(
	id_Proveedor BIGSERIAL NOT NULL,
	direccion varchar(50) NOT NULL,
	tipo varchar(50) NOT NULL,
	CONSTRAINT pk_Proveedor PRIMARY KEY(id_Proveedor)
);
DROP TABLE Empleado2
CREATE TABLE Empleado2
(
	RFC BIGSERIAL NOT NULL,
	nombre varchar(100) NOT NULL,	
	sueldo decimal NOT NULL,
	puesto varchar(30) NOT NULL,
	teléfono varchar(30) NOT NULL,	
	dirección varchar(30) NOT NULL,
	fecha_nacimiento date  NOT NULL,
	genero char not null,
	hora_entrada time not null,
	hora_salida time not null,
	id_Sucursal INT8 NOT NULL,
	CONSTRAINT RFC PRIMARY KEY(RFC)
);
DROP TABLE Venta2
CREATE TABLE Venta2
(
	id_Venta BIGSERIAL NOT NULL,
	fecha date NOT NULL,
	descripci�n varchar(50) NOT NULL,
	productos varchar(100) NOT NULL,
	cantidad int NOT NULL,
	total decimal NOT NULL,
	CONSTRAINT pk_Venta PRIMARY KEY(id_Venta)
);
drop table producto2
CREATE TABLE Producto2(
	id_Producto BIGSERIAL NOT NULL,
	nombre varchar(30) NOT NULL,
	precio real NOT NULL,
	caducidad date NOT NULL,
	cantidad INT NOT NULL,
	id_Proveedor INT8 NOT NULL,
	id_Venta INT8 ,
	CONSTRAINT pk_Producto PRIMARY KEY(id_Producto),
	CONSTRAINT fk_Proveedor FOREIGN KEY(id_Proveedor)
	REFERENCES Proveedor2(id_Proveedor)
);
drop table detalle_venta2
CREATE TABLE Detalle_Venta2(
	id_Detalle_Venta BIGSERIAL NOT NULL,
	id_Producto INT8 NOT NULL,
	id_Venta INT8 NOT NULL,
	CONSTRAINT pk_Detalle_Venta PRIMARY KEY(id_Detalle_Venta),
	CONSTRAINT fk_Producto FOREIGN KEY(id_Producto)
	REFERENCES Producto2(id_Producto),
	CONSTRAINT fk_Venta FOREIGN KEY(id_Venta)
	REFERENCES Venta2(id_Venta)
);


