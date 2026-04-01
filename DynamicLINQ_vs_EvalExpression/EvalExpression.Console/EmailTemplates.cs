namespace EvalExpression.Console;

public static class EmailTemplates
{
    public static string GetSimpleTemplate()
    {
        return @"
Hello {{Name}},

Thank you for your order.

Best regards,
The Team
";
    }
    
    public static string GetTemplate()
    {
        return @"
Dear {{Name}},

Thank you for your recent order with us! We are currently processing the following items:

<table>
<tr><td>Product Name</td><td>Price ($)</td><td>Quantity</td></tr>
{{
	var sb = new StringBuilder();
	foreach(var item in Items) {
		sb.AppendLine($$""""""<tr><td>{{item.Name}}</td><td>${{item.Price}}</td><td>{{item.Quantity}}</td></tr>"""""");
	}
	return sb.ToString();
}}
</table>

We will notify you once your order is shipped.

Best regards,
The Customer Service Team
";
    }
}