using System.Globalization;
using Budget.Repository;

namespace Budget;

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        var budgets = _budgetRepo.GetAll();

        var periods = GetPeriod(start, end);
        var total = 0m;
        foreach (var period in periods)
        {
            var budget = budgets.FirstOrDefault(x => x.YearMonth == period.Key);
            if (budget != null)
            {
                var days = GetDays(period.Key);
                var budgetAmount = budget.Amount / days;
                total += budgetAmount * period.Value;
            }
        }

        return total;
    }

    private static int GetDays(string periodKey)
    {
        var dateTime = DateTime.ParseExact(periodKey, "yyyyMM", CultureInfo.InvariantCulture);
        return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
    }

    private Dictionary<string, int> GetPeriod(DateTime start, DateTime end)
    {
        var dictionary = new Dictionary<string, int>();

        var current = start;
        while (current <= end)
        {
            var key = current.ToString("yyyyMM");
            if (dictionary.ContainsKey(key))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }

            current = current.AddDays(1);
        }

        return dictionary;
    }
}