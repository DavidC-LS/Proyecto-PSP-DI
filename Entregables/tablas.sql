CREATE DATABASE IF NOT EXISTS taller_di_psp CHARACTER SET utf8mb4;
USE taller_di_psp;

CREATE TABLE Clientes(
	id_clientes INT auto_increment PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	telefono VARCHAR(20),
	email VARCHAR(100)
);


CREATE TABLE Vehiculos(
    id_vehiculo INT AUTO_INCREMENT PRIMARY KEY,
    matricula VARCHAR(20) NOT NULL UNIQUE,
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

CREATE TABLE Incentivos(
    id_reparacion INT NOT NULL,
    id_mecanico INT NOT NULL,
    id_peticion CHAR(36) NOT NULL,
    fecha DATETIME NOT NULL,
    importe DECIMAL(11,2) NOT NULL,
    estado VARCHAR(30) NOT NULL,
    detalle VARCHAR(255) NOT NULL,
    PRIMARY KEY (id_reparacion, id_mecanico),
    UNIQUE (id_peticion),
    FOREIGN KEY (id_reparacion, id_mecanico)
        REFERENCES Reparacion_Mecanico(id_reparacion, id_mecanico)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- Datos ficticios para la demostración.
INSERT INTO Clientes (id_clientes, nombre, telefono, email) VALUES
    (1, 'Laura Demo', '600000001', 'laura@example.com'),
    (2, 'Pablo Demo', '600000002', 'pablo@example.com');
INSERT INTO Vehiculos (id_vehiculo, matricula, marca, modelo, id_clientes) VALUES
    (1, '1234 DMO', 'Seat', 'Ibiza', 1),
    (2, '5678 DMO', 'Renault', 'Clio', 2);
INSERT INTO Mecanicos (id_mecanico, nombre, especialidad) VALUES
    (1, 'Carlos García', 'Mecánica general'),
    (2, 'Ana Demo', 'Electricidad');
INSERT INTO Reparaciones (id_reparacion, id_vehiculo, fecha, descripcion, estado) VALUES
    (15, 1, '2026-10-20', 'Cambio de aceite y filtros', 'Finalizada'),
    (16, 2, '2026-10-21', 'Revisión del sistema eléctrico', 'En curso');
INSERT INTO Reparacion_Mecanico (id_reparacion, id_mecanico, horas_trabajadas, coste_hora) VALUES
    (15, 1, 2.50, 30.00), (15, 2, 1.00, 35.00), (16, 2, 1.50, 35.00);
INSERT INTO Facturas (id_factura, importe, fecha, resultado, id_reparacion) VALUES
    (25, 110.00, '2026-10-20', 'Pagada', 15),
    (26, 52.50, '2026-10-21', 'Pendiente', 16);
