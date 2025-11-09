using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PrimeVideoPaymentSimulator.Models;
using PrimeVideoPaymentSimulator.Data;
using PrimeVideoPaymentSimulator.Services;
using BCrypt.Net;
using System.Threading.Tasks;
using System.Linq; // Necesario para Enumerable.Range
using System; // Necesario para DateTime

namespace PrimeVideoPaymentSimulator.Pages
{
    // Clase que hereda correctamente de PageModel
    public class PaymentModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly CardValidationService _validator;

        public PaymentModel(AppDbContext context, CardValidationService validator)
        {
            _context = context;
            _validator = validator;
        }

        [BindProperty]
        public CardModel Card { get; set; } = new CardModel();

        // Propiedades para mensajes (CS8618 corregido con inicialización)
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        // MÉTODOS DE INICIALIZACIÓN DE VIEW DATA (CRÍTICO PARA EL ERROR ARGUMENTNULL)
        private void InitializeSelectLists()
        {
            // Inicializar el modelo con años y meses para los selectores
            // Usamos un tipo anónimo para que Razor lo pueda convertir fácilmente a SelectList
            ViewData["Months"] = Enumerable.Range(1, 12)
                .Select(m => new { Value = m, Text = m.ToString("00") }).ToList();

            ViewData["Years"] = Enumerable.Range(DateTime.Now.Year, 15)
                .Select(y => new { Value = y, Text = y.ToString() }).ToList();
        }

        public void OnGet()
        {
            InitializeSelectLists(); // Se ejecuta al cargar la página (soluciona el error inicial)
            Message = "Ingresa la información de tu tarjeta.";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            InitializeSelectLists(); // ¡CRÍTICO! Recargar listas si falla la validación del modelo

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, corrige los errores en el formulario.";
                return Page();
            }

            if (!_validator.Validate(Card))
            {
                ErrorMessage = "Error de validación de la tarjeta. Verifique los datos.";
                return Page();
            }

            try
            {
                // Hashear y limpiar campos
                Card.HashedCardNumber = BCrypt.Net.BCrypt.HashPassword(Card.CardNumber);
                Card.HashedCvv = BCrypt.Net.BCrypt.HashPassword(Card.Cvv);

                // Establecer como string.Empty para evitar CS8625 (Cannot convert null literal to non-nullable)
                Card.CardNumber = string.Empty;
                Card.Cvv = string.Empty;

                Card.IsValid = true;

                _context.ValidatedCards.Add(Card);
                await _context.SaveChangesAsync();

                Message = "✅ Tarjeta añadida y validada exitosamente. ¡Pago simulado aprobado!";
                ErrorMessage = string.Empty;
                ModelState.Clear();
                Card = new CardModel();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Error al procesar la tarjeta: {ex.Message}";
                Message = string.Empty;
            }

            return Page();
        }
    }
}