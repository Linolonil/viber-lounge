// using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale
{
    public class SaleRequestFromDataDto : IValidatableObject
    {
        [Required(ErrorMessage = "A data e hora inicial são obrigatórias")]
        [DataType(DataType.DateTime)]
        public DateTime InitialDateTime { get; set; }

        [Required(ErrorMessage = "A data e hora final são obrigatórias")]
        [DataType(DataType.DateTime)]
        public DateTime FinalDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var now = DateTime.Now;

            // Permite que FinalDateTime seja “menor” que InitialDateTime 
            // quando for no dia seguinte (ex.: 06:00 → 03:00 do dia+1)
            var effectiveFinal = FinalDateTime < InitialDateTime
                ? FinalDateTime.AddDays(1)
                : FinalDateTime;

            // Não pode estar no futuro
            if (InitialDateTime > now)
                yield return new ValidationResult(
                    "A data e hora inicial não podem ser no futuro",
                    new[] { nameof(InitialDateTime) });

            if (effectiveFinal > now)
                yield return new ValidationResult(
                    "A data e hora final não podem ser no futuro",
                    new[] { nameof(FinalDateTime) });

            // Limita o intervalo a, por exemplo, 90 dias
            var span = (effectiveFinal - InitialDateTime).TotalDays;
            if (span > 90)
                yield return new ValidationResult(
                    "O período máximo para consulta é de 90 dias",
                    new[] { nameof(InitialDateTime), nameof(FinalDateTime) });
        }
    }
}
