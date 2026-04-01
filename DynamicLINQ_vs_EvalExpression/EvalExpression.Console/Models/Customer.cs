namespace EvalExpression.Console.Models;

public record Customer
{
	public string Name { get; init; } = null!;

	public List<OrderItem> Items { get; init; } = [];
}

public record OrderItem
{
	public string Name { get; init; } = null!;

	public decimal Price { get; init; }

	public int Quantity { get; init; }
}
