using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale;

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

        // Não pode estar no futuro
        if (InitialDateTime > now)
            yield return new ValidationResult(
                "A data e hora inicial não podem ser no futuro",
                [nameof(InitialDateTime)]);

        if (FinalDateTime > now)
            yield return new ValidationResult(
                "A data e hora final não podem ser no futuro",
                [nameof(FinalDateTime)]);

        // Verifica se a data inicial é menor que a data final
        if (InitialDateTime > FinalDateTime)
            yield return new ValidationResult(
                "A data e hora inicial deve ser menor que a data e hora final",
                [nameof(InitialDateTime), nameof(FinalDateTime)]);

        // Limita o intervalo a, por exemplo, 90 dias
        var span = (FinalDateTime - InitialDateTime).TotalDays;
        if (span > 90)
            yield return new ValidationResult(
                "O período máximo para consulta é de 90 dias",
                [nameof(InitialDateTime), nameof(FinalDateTime)]);
    }
}
