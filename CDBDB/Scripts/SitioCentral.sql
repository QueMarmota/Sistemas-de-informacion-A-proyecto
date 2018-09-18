Create DATABASE SitioCentral
use SitioCentral
Select * from fragmentos
CREATE TABLE Fragmentos(
	id_Fragmento BIGINT IDENTITY(1,1) NOT NULL,
	nombre varchar(30) NOT NULL,
	tabla varchar(30) NOT NULL,
	tipo varchar(30) NOT NULL,
	sitio varchar(10) NOT NULL,
	condición varchar(30) not null,
	
	constraint pk_Fragmento primary key (id_Fragmento)
);
INSERT INTO Fragmentos values('Proveedor1','Proveedor','Vertical','1','');
INSERT INTO Fragmentos values('Proveedor2','Proveedor','Vertical','2','');
INSERT INTO Fragmentos values('Empleado1','Empleado','Horizontal','1','Puesto = "Empleado"');
INSERT INTO Fragmentos values('Empleado2','Empleado','Horizontal','2','Puesto = "Administrador"');
INSERT INTO Fragmentos values('Producto1','Producto','Replica','1','');
INSERT INTO Fragmentos values('Producto2','Producto','Replica','2','');
INSERT INTO Fragmentos values('Sucursal1','Sucursal','','1','');
INSERT INTO Fragmentos values('Suministra2','Suministra','','2','');
INSERT INTO Fragmentos values('Oferta1','Oferta','','1','');
INSERT INTO Fragmentos values('Tiene_Of1','Tiene_Of','','1','');
INSERT INTO Fragmentos values('Venta2','Venta','','2','');
INSERT INTO Fragmentos values('Detalle_Venta2','Detalle_Venta','','2','');

CREATE TABLE Atributos(
	id_Atributo INT NOT NULL,
	nombre varchar(30) NOT NULL
);
select * from Atributos
INSERT INTO Atributos values('1','id_Proveedor');
INSERT INTO Atributos values('1','nombre');
INSERT INTO Atributos values('1','email');
INSERT INTO Atributos values('1','teléfono');
INSERT INTO Atributos values('2','id_Proveedor');
INSERT INTO Atributos values('2','direccion');
INSERT INTO Atributos values('2','tipo');
INSERT INTO Atributos values('3','RFC');
INSERT INTO Atributos values('3','nombre');
INSERT INTO Atributos values('3','sueldo');
INSERT INTO Atributos values('3','puesto');
INSERT INTO Atributos values('3','teléfono');
INSERT INTO Atributos values('3','dirección');
INSERT INTO Atributos values('3','fecha_nacimiento');
INSERT INTO Atributos values('3','genero');
INSERT INTO Atributos values('3','hora_entrada');
INSERT INTO Atributos values('3','hora_salida');
INSERT INTO Atributos values('3','id_Sucursal');
INSERT INTO Atributos values('4','RFC');
INSERT INTO Atributos values('4','nombre');
INSERT INTO Atributos values('4','sueldo');
INSERT INTO Atributos values('4','puesto');
INSERT INTO Atributos values('4','teléfono');
INSERT INTO Atributos values('4','dirección');
INSERT INTO Atributos values('4','fecha_nacimiento');
INSERT INTO Atributos values('4','genero');
INSERT INTO Atributos values('4','hora_entrada');
INSERT INTO Atributos values('4','hora_salida');
INSERT INTO Atributos values('4','id_Sucursal');
INSERT INTO Atributos values('5','id_Producto');
INSERT INTO Atributos values('5','nombre');
INSERT INTO Atributos values('5','caducidad');
INSERT INTO Atributos values('5','cantidad');
INSERT INTO Atributos values('5','id_Proveedor');
INSERT INTO Atributos values('5','id_Venta');
INSERT INTO Atributos values('6','id_Producto');
INSERT INTO Atributos values('6','nombre');
INSERT INTO Atributos values('6','caducidad');
INSERT INTO Atributos values('6','cantidad');
INSERT INTO Atributos values('6','id_Proveedor');
INSERT INTO Atributos values('6','id_Venta');