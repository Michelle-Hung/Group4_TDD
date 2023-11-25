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
        var daysOfMonthList = GetDaysOfMonth(start, end);
        var total = 0m;
        foreach (var daysOfMonth in daysOfMonthList)
        {
            var budget = budgets.FirstOrDefault(x => x.YearMonth == daysOfMonth.Key);
            if (budget == null) continue;
            var days = GetDays(daysOfMonth.Key);
            var budgetAmount = (decimal)budget.Amount / days;
            total += budgetAmount * daysOfMonth.Value;
        }

        return total;
    }

    private static int GetDays(string periodKey)
    {
        var dateTime = DateTime.ParseExact(periodKey, "yyyyMM", CultureInfo.InvariantCulture);
        return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
    }

    private Dictionary<string, int> GetDaysOfMonth(DateTime start, DateTime end)
    {
        var periodMapping = new Dictionary<string, int>();
        var current = start;
        while (current <= end)
        {
            var key = current.ToString("yyyyMM");
            if (periodMapping.ContainsKey(key))
            {
                periodMapping[key]++;
            }
            else
            {
                periodMapping.Add(key, 1);
            }

            current = current.AddDays(1);
        }

        return periodMapping;
    }
}