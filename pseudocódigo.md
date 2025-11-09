# 📝 Flujo de Ejecución General del Programa

Este documento presenta el pseudocódigo que describe el flujo de ejecución completo del **PrimeVideoPaymentSimulator**, ilustrando cómo la lógica del servidor (Code-Behind) interactúa con la vista y los servicios de seguridad y validación.

---

### 1. Flujo de Solicitud Inicial (Carga de la Página)

Este proceso ocurre cuando el usuario accede por primera vez a la ruta `/Payment` (método **HTTP GET**).

```pseudocode
// Evento: Usuario navega a /Payment (Solicitud HTTP GET)

FUNCIÓN RequestHandler_GET:
    // 1.1 Inicialización del Modelo:
    INSTANCIAR PaymentModel (Con Inyección de Dependencias de AppDbContext y CardValidationService)

    // 1.2 Ejecución de Lógica de Carga (Herencia de PageModel):
    PaymentModel.OnGet():
        LLAMAR a InitializeSelectLists() // Carga las listas de Mes/Año en ViewData.
    FIN OnGet

    // 1.3 Renderizado y Respuesta:
    RENDERIZAR Payment.cshtml (La Vista Razor, enlazada a PaymentModel)
    ENVIAR el HTML resultante al Navegador.
    // El Navegador muestra el formulario vacío y las listas desplegables.
FIN FUNCIÓN


---
// Evento: Usuario presiona "Añade tu tarjeta" (Solicitud HTTP POST)

FUNCIÓN PaymentModel.OnPostAsync:

    // 2.1 Mapeo y Recarga:
    RECIBIR Datos del Formulario y MAPEARLOS al objeto CardModel (Binding).
    LLAMAR a InitializeSelectLists() // Recarga Mes/Año en caso de que la validación posterior falle.

    // 2.3 Validación 1: Estructura (Data Annotations en C#):
    SI ModelState NO es VÁLIDO EN C# (Verifica [Required], [Length], etc. de CardModel) ENTONCES
        Mostrar Errores de Formato/Requerido.
        DEVOLVER PÁGINA
    FIN SI

    // 2.4 Validación 2: Lógica de Negocio (Abstracción/Servicio):
    SI CardValidationService.Validate(Card) NO es VERDADERO ENTONCES
        Mostrar Error Específico (ej. "Tarjeta caducada").
        DEVOLVER PÁGINA
    FIN SI

    // 2.5 SEGURIDAD: Proceso de Hashing (BCrypt):
    ASIGNAR Card.HashedCardNumber = BCrypt.HashPassword(Card.CardNumber)
    ASIGNAR Card.HashedCvv = BCrypt.HashPassword(Card.Cvv)

    // 2.6 SEGURIDAD: Limpieza de Valores Sensibles:
    ASIGNAR Card.CardNumber = CADENA VACÍA
    ASIGNAR Card.Cvv = CADENA VACÍA

    // 2.7 Persistencia (EF Core):
    _context.ValidatedCards.ADD(Card)
    GUARDAR cambios en la base de datos (_context.SaveChangesAsync)

    // 2.8 Respuesta Final:
    Mostrar Mensaje de Éxito: "Pago simulado aprobado."
    REINICIAR formulario.
    DEVOLVER PÁGINA (Con formulario limpio y mensaje de éxito)

FIN FUNCIÓN
