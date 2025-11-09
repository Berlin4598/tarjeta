using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeVideoPaymentSimulator.Models
{
    public class CardModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Propiedades de entrada (inicializadas para eliminar CS8618)
        [Required(ErrorMessage = "El nombre del titular es requerido.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        [Display(Name = "Nombre del titular de la tarjeta")]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de tarjeta es requerido.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta debe contener solo 16 dígitos.")]
        [Display(Name = "Número de tarjeta")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CVV es requerido.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El CVV debe tener 3 dígitos.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "El CVV debe contener solo 3 dígitos.")]
        [Display(Name = "Código de seguridad (CVV)")]
        public string Cvv { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mes de vencimiento es requerido.")]
        [Range(1, 12, ErrorMessage = "El mes debe ser entre 01 y 12.")]
        [Display(Name = "Mes de vencimiento")]
        public int ExpiryMonth { get; set; }

        [Required(ErrorMessage = "El año de vencimiento es requerido.")]
        [Range(2023, 2035, ErrorMessage = "El año debe ser válido (ej. entre 2023 y 2035).")]
        [Display(Name = "Año de vencimiento")]
        public int ExpiryYear { get; set; }

        // Propiedades hasheadas (permiten ser nulas con '?')
        public string? HashedCardNumber { get; set; }
        public string? HashedCvv { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public bool IsValid { get; set; } = false;
    }
}