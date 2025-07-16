# Desarrollo del Sistema "HuecasYa". Recomendaciones de Almuerzo para Estudiantes de la ASEUDLA FICA.

## Aplicación móvil multiplataforma llamada HuecasYa, orientada a los estudiantes de la ASEUDLA FICA, que permita comparar opciones gastronómicas cercanas según sus preferencias alimenticias, presupuesto, horarios y situaciones sociales específicas, con el fin de optimizar su experiencia alimentaria durante la jornada académica y mejorar su bienestar en el campus UDLA Park.

## Consideraciones técnicas

## Tecnología principal usada
### Se utilizó .NET MAUI, una plataforma de desarrollo que permite crear aplicaciones móviles multiplataforma (Android e iOS) usando C# y XAML. Es ideal para proyectos como HuecasYa, ya que con una sola base de código se pueden generar apps para varios dispositivos.
## Prerrequisitos necesarios:
### Antes de desarrollar con .NET MAUI, fue necesario instalar lo siguiente:
### - Visual Studio (el entorno de desarrollo).
### - SDK de .NET 6 (el kit de desarrollo necesario para compilar y ejecutar la app).
### - Carga de trabajo de MAUI, que se instala desde el instalador de Visual Studio.
## Backend y servicios usados:
### - Se utilizó el framework ASP.NET Core Web API para crear el backend del sistema, con lógica y servicios que se comunican con la app.
### - La comunicación entre el frontend (la app) y el backend se hace usando API REST, enviando y recibiendo datos en formato JSON.
## Base de datos:
### Se eligió MongoDB, una base de datos NoSQL ideal para estructuras de datos flexibles, como menús o preferencias. Se manejó a través del driver oficial MongoDB.Driver, que permite trabajar con MongoDB en C#.
## Servidor web:
### Se usó Kestrel, que es el servidor web predeterminado en aplicaciones ASP.NET Core. Se encarga de recibir las peticiones HTTP y responder desde el backend.
## Comunicación entre servicios:
### Para consumir APIs externas o internas, se usó HttpClient, que es una librería estándar en C# para enviar peticiones HTTP y manejar respuestas, generalmente en formato JSON.
