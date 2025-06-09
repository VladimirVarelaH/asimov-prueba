# Proyecto Asimov - Prueba Técnica

Este proyecto es una aplicación fullstack compuesta por un backend en .NET 8, una base de datos SQLite y un frontend desarrollado con Vue.js.  
El proyecto está disponible con un cliente desplegado en [asimov-front.asociate.cl/](https://asimov-front.asociate.cl/) y el backend en [asimov-back.asociate.cl/](https://asimov-back.asociate.cl/).  
![UI](https://github.com/VladimirVarelaH/asimov-prueba/blob/main/docu/image.jpg?raw=true)


## Tecnologías

- **Backend:** .NET 8
- **Base de datos:** SQLite
- **Frontend:** Vue.js
- **Orquestación:** Docker Compose

## Estructura del Proyecto

- `/backend`: Código fuente del backend (.NET 8)
- `/frontend`: Código fuente del cliente Vue.js
- `/database`: Archivos y scripts relacionados con SQLite
- `docker-compose.yml`: Archivo de orquestación de contenedores

## Requisitos

- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/)

## Instalación

1. **Clonar el repositorio:**
    ```bash
    git clone <url-del-repositorio>
    cd <nombre-del-repositorio>
    ```

2. **Levantar la aplicación con Docker Compose:**
    ```bash
    docker-compose up --build
    ```

Esto iniciará tanto el backend como el frontend y la base de datos en contenedores separados.

### Ejecución si Docker
Si se quiere ejecutar sin utilizar Docker es importante ejecutar las migraciones con el comando `dotnet ef database update` y después levantar el servicio con `dotnet run`

## Uso
- El backend estará disponible en `http://localhost:5000` (o el puerto configurado).
- El frontend estará disponible en `http://localhost:8080`.

### Endpoints de la API
#### `GET /api/appointment/test-db`
Prueba de conexión con la base de datos.

**Respuesta exitosa**
```json
{
  "DatabaseConnected": true
}
```

**Respuesta de error**
```json
{
  "DatabaseConnected": false,
  "Error": "Mensaje de error"
}
```

#### `GET /api/appointment`
Obtiene todas las citas agendadas.


**Respuesta**
```json
[
  {
    "id": 1,
    "date": "2025-06-09",
    "startTime": "10:00"
  },
  ...
]
```
#### `POST /api/appointment`
Crea una nueva cita.

**Restricciones**

- Solo se permiten citas de lunes a viernes.
- Horario permitido: entre 09:00 y 18:00.
- No se permiten citas con menos de 1 hora de diferencia entre sí el mismo día.

**Cuerpo del request**
```json
{
  "date": "2025-06-05",
  "startTime": "13:00",
  "contactEmail": "someone@example.com"
}
```

#### `GET /api/appointment/taken?year={year}&month={month}`
Obtiene las horas ya reservadas para un mes específico.

**Parámetros**

- `year`: Año numérico (ej: `2025`)

- `month`: Mes numérico (1-12)

**Ejemplo**
`GET /api/appointment/taken?year=2025&month=6`

**Respuesta**
```json
{
  "2025-06-09": ["10:00", "14:00"],
  "2025-06-10": ["09:00"]
}
```

**Respuestas Posibles**
- `200 OK`: Datos obtenidos correctamente.
- `400 Bad Request`: Si los parámetros son inválidos.

