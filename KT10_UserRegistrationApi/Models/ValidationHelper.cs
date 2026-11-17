using System.ComponentModel.DataAnnotations;

namespace KT10_UserRegistrationApi.Models;

public static class ValidationHelper
{
    public static Dictionary<string, string[]> Validate<T>(T model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model!);

        Validator.TryValidateObject(model!, context, results, true);

        return results
            .SelectMany(result =>
                (result.MemberNames.Any()
                    ? result.MemberNames
                    : new[] { "request" })
                .Select(member => new
                {
                    Member = member,
                    Error = result.ErrorMessage ?? "Некорректное значение"
                }))
            .GroupBy(x => x.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.Error).ToArray());
    }
}