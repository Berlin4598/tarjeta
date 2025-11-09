# 🛡️ PrimeVideoPaymentSimulator: Aplicación Web Segura con POO

Este proyecto es una aplicación web desarrollada con **ASP.NET Core Razor Pages** que simula un formulario de pago al estilo de Prime Video. El objetivo es demostrar la aplicación rigurosa de principios de la **Programación Orientada a Objetos (POO)**, el uso de **Inyección de Dependencias** y la implementación de técnicas de **cifrado (BCrypt)** para la protección de datos sensibles.

---

## ⚙️ Tecnologías Implementadas

| Tecnología | Rol y Aplicación en el Proyecto |
| :--- | :--- |
| **ASP.NET Core Razor Pages** | Framework utilizado para la creación de la interfaz de usuario (Front-end) y el *code-behind* (Back-end) en un modelo de **Page Model (MVVM)**, garantizando la separación de la lógica (`Payment.cshtml.cs`) y la presentación (`Payment.cshtml`). |
| **Programación Orientada a Objetos (POO)** | Estructura principal. Utiliza **Clases** (`CardModel`, `PaymentModel`) y **Servicios** (`CardValidationService`). Se aplica **Inyección de Dependencias** para facilitar el acceso a la base de datos y la lógica de validación. |
| **Entity Framework Core (EF Core)** | ORM utilizado para gestionar la base de datos en memoria (`UseInMemoryDatabase`), permitiendo la persistencia simulada de los *hashes* de las tarjetas validadas. |
| **BCrypt.NET** | Implementación del algoritmo de *hashing* adaptativo. Se usa para **hashear el número de tarjeta y el CVV** antes de guardarlos. Esto asegura que la base de datos **nunca** contenga los datos sensibles en texto plano (principio de **seguridad por diseño**). |
| **Validación Dual (C#/JavaScript)** | **C# Data Annotations** y la clase `CardValidationService` garantizan la integridad de los datos en el servidor (validación de reglas y fecha de caducidad), mientras que **jQuery/JavaScript** ofrece una mejor experiencia de usuario (UX) restringiendo la entrada de caracteres en tiempo real. |

---

## 🔑 Flujo y Lógica del Servidor (`Payment.cshtml.cs`)

La lógica de seguridad y validación se ejecuta completamente en el método `OnPostAsync()` del Page Model.

### Pasos Ejecutados en el Servidor:

1.  **Inicialización (`OnGet`):** Al cargar la página, se ejecuta `OnGet()` para inicializar las listas de Meses y Años en `ViewData`.
2.  **Recepción y Validación Inicial:** Al enviar el formulario, `OnPostAsync` recibe el objeto `CardModel` y verifica:
    * **Data Annotations:** Verifica que se cumplan las reglas básicas (`[Required]`, `[StringLength]`, `[RegularExpression]`) definidas en el modelo.
    * **Lógica de Negocio:** Llama a `CardValidationService.Validate(Card)` para comprobar reglas complejas (ej. la tarjeta no está expirada).
3.  **Proceso de Hashing:** Si todas las validaciones pasan:
    * Se utiliza `BCrypt.HashPassword()` para generar el *hash* criptográfico del `CardNumber` y `Cvv`.
    * Los *hashes* resultantes se almacenan en las propiedades `HashedCardNumber` y `HashedCvv`.
4.  **Limpieza de Datos Sensibles:** Los campos originales (`CardNumber` y `Cvv`) se **limpian (`= string.Empty;`)** antes de cualquier intento de guardar.
5.  **Persistencia:** La instancia de `CardModel` (que solo contiene *hashes* y datos no sensibles) es añadida al contexto y guardada en la base de datos en memoria (`_context.ValidatedCards.Add(Card)`).

---

## 📐 Principios de la Programación Orientada a Objetos (POO)

El diseño del proyecto está construido sobre los pilares de la POO:

### 1. Encapsulamiento
El encapsulamiento se utiliza para proteger la información interna y la lógica de las clases:

* **Variables Privadas (`private readonly`):** En la clase `PaymentModel.cs`, las dependencias inyectadas (`_context` y `_validator`) se declaran como `private readonly`. Esto significa que **solo los métodos internos** de la clase `PaymentModel` pueden acceder a ellos. Esto protege el contexto de la base de datos y el servicio de validación de modificaciones externas e involuntarias.
* **Propiedades Controladas:** Las propiedades de `CardModel.cs` utilizan *getters* y *setters* para controlar el acceso, manteniendo la integridad de los datos.

### 2. Herencia
La herencia se utiliza para obtener funcionalidades predefinidas del framework:

* **`PaymentModel`:** Hereda de **`PageModel`** (librería `Microsoft.AspNetCore.Mvc.RazorPages`). Esta herencia otorga al modelo los métodos fundamentales para manejar las peticiones web (`OnGet` para GET y `OnPostAsync` para POST), permitiendo que el código se enfoque en la lógica de la aplicación y no en el protocolo HTTP.
* **`AppDbContext`:** Hereda de **`DbContext`** (Entity Framework Core), obteniendo la capacidad de mapear objetos C# a la base de datos.

### 3. Abstracción
La abstracción se aplica para simplificar la complejidad del sistema, mostrando solo lo esencial:

* **Servicio de Validación (`CardValidationService`):** Esta clase es el ejemplo clave de abstracción. Para la clase `PaymentModel`, la validación de la tarjeta es un proceso simple: llama a `_validator.Validate(Card)` y obtiene `true` o `false`.
    * **Lo Abstraído (Oculto):** El `PaymentModel` no necesita saber *cómo* el servicio comprueba la fecha actual, ni *cómo* compara el año de vencimiento. La complejidad de la lógica de negocio (el "cómo") está oculta dentro del servicio.
    * **Beneficio:** Permite modificar la lógica de validación del servicio sin tener que tocar el código del Page Model, mejorando la modularidad y el mantenimiento.

---

### 🛡️ Seguridad de Datos y Persistencia

* **BCrypt para Hashing:** En `OnPostAsync()`, el número de tarjeta y el CVV se pasan a `BCrypt.HashPassword()`. Este algoritmo crea un *hash* unidireccional (imposible de revertir) que se guarda en las propiedades `HashedCardNumber` y `HashedCvv`.
* **Limpieza de Datos Sensibles:** Antes de que Entity Framework Core guarde el modelo, los campos originales (`CardNumber` y `Cvv`) se establecen explícitamente en **`string.Empty`**. Esto asegura que los valores en texto plano nunca lleguen a la base de datos, incluso si se trata de una base de datos en memoria.

---
