using PrimeVideoPaymentSimulator.Models;
using System;
using System.Linq;

namespace PrimeVideoPaymentSimulator.Services
{
    public class CardValidationService
    {
        private const int CardNumberLength = 16;
        private const int CvvLength = 3;

        public bool Validate(CardModel card)
        {
            // Las validaciones de longitud y dígitos ya están cubiertas por Data Annotations,
            // pero se mantienen las verificaciones de lógica de negocio (como la fecha).

            // 1. Validar Fecha de Expiración
            if (!IsExpiryDateValid(card.ExpiryMonth, card.ExpiryYear))
            {
                return false;
            }

            // 2. Comprobación final de los datos (Longitud/Dígitos)
            if (string.IsNullOrWhiteSpace(card.CardNumber) ||
                card.CardNumber.Length != CardNumberLength ||
                !card.CardNumber.All(char.IsDigit))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(card.Cvv) ||
                card.Cvv.Length != CvvLength ||
                !card.Cvv.All(char.IsDigit))
            {
                return false;
            }

            return true;
        }

        private bool IsExpiryDateValid(int month, int year)
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            if (year < currentYear) return false;

            if (year == currentYear && month < currentMonth) return false;

            if (month < 1 || month > 12) return false;

            return true;
        }
    }
}