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
        if (start.Year == end.Year && start.Month == end.Month)
        {
            var budget = budgets.SingleOrDefault(x => x.YearMonth == start.ToString("yyyyMM"));
            if (budget != null)
            {
                var days = (end - start).Days + 1;
                return budget.Amount /(decimal)DateTime.DaysInMonth(start.Year, start.Month) * days;
            }
        }
        else
        {
            
            
        }

        return 0m;
    }
}