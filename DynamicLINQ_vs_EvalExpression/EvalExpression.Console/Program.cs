using Bogus;
using EvalExpression.Console;
using EvalExpression.Console.Models;
using Z.Expressions;

var orderItems = new Faker<OrderItem>()
    .RuleFor(o => o.Name, f => f.Commerce.ProductName())
    .RuleFor(o => o.Price, f => decimal.Parse(f.Commerce.Price().Replace("$", "").Replace(",", "").Replace(".", "")) / 100m)
    .RuleFor(o => o.Quantity, f => f.Random.Int(1, 10))
    .Generate(5);

var customer = new Faker<Customer>()
	.RuleFor(o => o.Name, f => f.Person.FirstName)
	.RuleFor(o => o.Items, _ => orderItems)
	.Generate();

var simpleEmailTemplate = EmailTemplates.GetSimpleTemplate();
var emailTemplate = EmailTemplates.GetTemplate();

var emailResult = Eval.Execute<string>($"return $$'''{simpleEmailTemplate}'''", customer);
var emailResult2 = Eval.Execute<string>($"return $$'''{emailTemplate}'''", customer);

Console.WriteLine(emailResult);

Console.WriteLine("\n\n\n");

Console.WriteLine(emailResult2);