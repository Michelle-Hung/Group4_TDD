using Budget;
using Budget.Repository;
using NSubstitute;

namespace TestProject1;

[TestFixture]
public class Tests
{
    private IBudgetRepo _budgetRepo = null!;
    private BudgetService _service = null!;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _service = new BudgetService(_budgetRepo);
    }

    [Test]
    public void one_day()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            }
        });
        var actual = _service.Query(new DateTime(2023, 11, 25), new DateTime(2023, 11, 25));

        TotalBudgetShouldBe(10m, actual);
    }

    [Test]
    public void one_month()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            }
        });
        var actual = _service.Query(new DateTime(2023, 11, 1), new DateTime(2023, 11, 30));

        TotalBudgetShouldBe(300m, actual);
    }

    [Test]
    public void cross_month()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            },
            new()
            {
                YearMonth = "202310",
                Amount = 620
            }
        });
        var actual = _service.Query(new DateTime(2023, 10, 28), new DateTime(2023, 11, 3));

        TotalBudgetShouldBe(110m, actual);

    }

    [Test]
    public void cross_month_no_budget()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            },
            new()
            {
                YearMonth = "202310",
                Amount = 620
            }
        });
        var actual = _service.Query(new DateTime(2023, 09, 28), new DateTime(2023, 11, 3));

        TotalBudgetShouldBe(650m, actual);
    }

    [Test]
    public void same_month_no_budget()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>());
        var actual = _service.Query(new DateTime(2023, 09, 28), new DateTime(2023, 9, 30));

        TotalBudgetShouldBe(0m, actual);
    }

    [Test]
    public void cross_year()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new Budget.Repository.Budget
            {
                YearMonth = "202312",
                Amount = 310
            },
            new Budget.Repository.Budget
            {
                YearMonth = "202401",
                Amount = 930
            }
        });
        var actual = _service.Query(new DateTime(2023, 12, 28), new DateTime(2024, 1, 5));

        TotalBudgetShouldBe(190m, actual);
    }

    [Test]
    public void leap_year()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new Budget.Repository.Budget
            {
                YearMonth = "202002",
                Amount = 290
            },
        });
        var actual = _service.Query(new DateTime(2020, 02, 20), new DateTime(2020, 2, 29));

        TotalBudgetShouldBe(100m, actual);
    }

    [Test]
    public void invalid_period()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new()
            {
                YearMonth = "202002",
                Amount = 290
            },
        });
        var actual = _service.Query(new DateTime(2020, 02, 29), new DateTime(2020, 2, 20));

        TotalBudgetShouldBe(0m, actual);
    }

    private static void TotalBudgetShouldBe(decimal expected, decimal actual)
    {
        Assert.That(actual, Is.EqualTo(expected));
    }


    private void GivenAllBudgets(List<Budget.Repository.Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}