using System.ComponentModel.DataAnnotations.Schema;
using Godot;

[GlobalClass]
public partial class GameRound : Resource
{
    [Export] CustomerData[] customers;

	public GameRound() : this([]) {}
	public GameRound(CustomerData[] Customers)
    {
        customers = Customers;
    }

    public CustomerData GetCustomerAtIndex(int index)
    {
        if (customers.Length <= index)
        {
            return null;
        }

        return customers[index];
    }
}
