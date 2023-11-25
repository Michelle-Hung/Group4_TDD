namespace Budget.Repository;

public interface IBudgetRepo
{
    List<BudgetDto> GetAll();
}