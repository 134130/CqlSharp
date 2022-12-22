using CqlSharp.Expressions;
using CqlSharp.Expressions.Literals;
using CqlSharp.Expressions.Predicate;
using CqlSharp.Query;

namespace CqlSharp.Tests;

public class Test_Custom_Select
{
    [Test]
    public async Task NoLimit()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task Limit()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            Limit = 1000,
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task Select1Column()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id")
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task SelectReverse()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("profession"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("id")
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task Where_Firstname_Apercent()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            WhereExpression = new PredicateExpression(
                new QualifiedIdentifier("firstname"),
                new PredicateLikeOperation("A%")
            ),
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task Where_Firstname_Apercent_And_Lastname_Apercent()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            WhereExpression =
                new AndOrExpression(BooleanLiteral.True, AndOrType.And, BooleanLiteral.True),
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task Where_Firstname_Apercent_Or_Lastname_Apercent()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            WhereExpression = new AndOrExpression(BooleanLiteral.True, AndOrType.Or, BooleanLiteral.True),
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task OrderById()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            OrderBys = new List<OrderBy>
            {
                new() { Column = new QualifiedIdentifier("id") }
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task OrderByIdDesc()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            OrderBys = new List<OrderBy>
            {
                new() { Column = new QualifiedIdentifier("id"), SortType = SortType.Desc }
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task OrderByFirstname()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            OrderBys = new List<OrderBy>
            {
                new() { Column = new QualifiedIdentifier("firstname") }
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task OrderByFirstnameDesc()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            OrderBys = new List<OrderBy>
            {
                new() { Column = new QualifiedIdentifier("firstname") }
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }

    [Test]
    public async Task OrderByProfession_Firstname_Lastname()
    {
        var selectQuery = new Select
        {
            Columns = new List<IColumn>
            {
                new QualifiedIdentifier("id"),
                new QualifiedIdentifier("firstname"),
                new QualifiedIdentifier("lastname"),
                new QualifiedIdentifier("email"),
                new QualifiedIdentifier("profession")
            },
            OrderBys = new List<OrderBy>
            {
                new() { Column = new QualifiedIdentifier("profession") },
                new() { Column = new QualifiedIdentifier("firstname") },
                new() { Column = new QualifiedIdentifier("lastname") }
            },
            From = new CsvTableReference("/Users/cooper/development/CQLSharp/csv/test1.csv")
        };
        await CsvSelectService.ProcessAsync(selectQuery);
    }
}