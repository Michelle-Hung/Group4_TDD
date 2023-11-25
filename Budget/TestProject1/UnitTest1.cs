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

        Assert.That(actual, Is.EqualTo(10m));
    }

    [Test]
    public void one_month()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new Budget.Repository.Budget
            {
                YearMonth = "202311",
                Amount = 300
            }
        });
        var actual = _service.Query(new DateTime(2023, 11, 1), new DateTime(2023, 11, 30));

        Assert.That(actual, Is.EqualTo(300m));
    }

    [Test]
    public void cross_month()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new Budget.Repository.Budget
            {
                YearMonth = "202311",
                Amount = 300
            },
            new Budget.Repository.Budget
            {
                YearMonth = "202310",
                Amount = 620
            }
        });
        var actual = _service.Query(new DateTime(2023, 10, 28), new DateTime(2023, 11, 3));

        Assert.That(actual, Is.EqualTo(110m));
    }

    [Test]
    public void cross_month_no_budget()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>
        {
            new Budget.Repository.Budget
            {
                YearMonth = "202311",
                Amount = 300
            },
            new Budget.Repository.Budget
            {
                YearMonth = "202310",
                Amount = 620
            }
        });
        var actual = _service.Query(new DateTime(2023, 09, 28), new DateTime(2023, 11, 3));

        Assert.That(actual, Is.EqualTo(650m));
    }

    [Test]
    public void same_month_no_budget()
    {
        GivenAllBudgets(new List<Budget.Repository.Budget>());
        var actual = _service.Query(new DateTime(2023, 09, 28), new DateTime(2023, 9, 30));

        Assert.That(actual, Is.EqualTo(0m));
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

        Assert.That(actual, Is.EqualTo(190m));
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

        Assert.That(actual, Is.EqualTo(100m));
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

        Assert.That(actual, Is.EqualTo(0m));
    }


    private void GivenAllBudgets(List<Budget.Repository.Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}