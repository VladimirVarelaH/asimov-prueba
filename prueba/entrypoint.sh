#!/bin/bash

if [ -f "appointments.db" ]; then
  echo "Eliminando appointments.db antigua..."
  rm appointments.db
fi

# Aplicar migraciones, especificando el proyecto
dotnet ef database update --project prueba.csproj --context AppDbContext

# Correr la app
dotnet prueba.dll
