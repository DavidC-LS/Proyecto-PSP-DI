

CREATE TABLE Clientes(
	id_clientes INT auto_increment PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	telefono VARCHAR(20),
	email VARCHAR(100)
);


CREATE TABLE Vehiculos(
    id_vehiculo INT AUTO_INCREMENT PRIMARY KEY,
    matricula VARCHAR(20) NOT NULL,
    marca VARCHAR(50) NOT NULL,
    modelo VARCHAR(50) NOT NULL,
    id_clientes INT NOT NULL,
    FOREIGN KEY (id_clientes) REFERENCES Clientes(id_clientes) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Mecanicos(
    id_mecanico INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    especialidad VARCHAR(100) NOT NULL
);

CREATE TABLE Reparaciones(
    id_reparacion INT AUTO_INCREMENT PRIMARY KEY,
    id_vehiculo INT NOT NULL,
    fecha DATE NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado VARCHAR(30) NOT NULL,
    FOREIGN KEY (id_vehiculo) REFERENCES Vehiculos(id_vehiculo) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Reparacion_Mecanico(
    id_reparacion INT NOT NULL,
    id_mecanico INT NOT NULL,
    horas_trabajadas DECIMAL(5,2) NOT NULL,
    coste_hora DECIMAL(8,2) NOT NULL,
    PRIMARY KEY (id_reparacion, id_mecanico),
    FOREIGN KEY (id_reparacion) REFERENCES Reparaciones(id_reparacion) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (id_mecanico) REFERENCES Mecanicos(id_mecanico) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Facturas(
    id_factura INT AUTO_INCREMENT PRIMARY KEY,
    importe DECIMAL(5,2) NOT NULL, 
    fecha DATE NOT NULL,
    resultado VARCHAR(30),
    id_reparacion INT NOT NULL,
    FOREIGN KEY (id_reparacion) REFERENCES Reparaciones(id_reparacion) ON DELETE CASCADE ON UPDATE CASCADE
);
