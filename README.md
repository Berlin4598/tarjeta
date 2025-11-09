# 💳 PrimeVideoPaymentSimulator: Simulador de Pago Seguro (ASP.NET Core)

Este proyecto es una aplicación web desarrollada con **ASP.NET Core Razor Pages** que simula la interfaz de pago de Prime Video. El objetivo principal es demostrar la aplicación de la **Programación Orientada a Objetos (POO)**, la **Inyección de Dependencias**, y la implementación de técnicas de **seguridad criptográfica (BCrypt)** para proteger la información sensible de tarjetas.

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
