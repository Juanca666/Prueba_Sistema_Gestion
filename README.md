# Prueba_Sistema_Gestion
Sistema de Gestión para Empresa de Distribución de películas
Aplicación para administrar películas con categorías, precios en USD y COP, desarrollada en .NET 8 con Clean Architecture y Docker.

#Requisitos
Docker Desktop instalado

#Instalacion
git clone https://github.com/Juanca666/Prueba_Sistema_Gestion.git
cd Prueba_Sistema_Gestion
docker-compose up --build

#Acesos
Frontend: http://localhost:5000
Swagger: http://localhost:5000/swagger
Base de datos: localhost,1433 (sa / SqlServer2026!)

#Funcionalidades
CRUD de Categorías y Películas
Conversión automática USD → COP
Logs de creación y modificación
Datos de prueba precargados
API documentada con Swagger
Frontend básico funcional
