using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GarnishObserver : Area2D
{
	public List<GarnishData> GetAllGarnishDataInGlass()
	{
		var overlappingBodies = GetOverlappingBodies();

		var garnishes = overlappingBodies
			.Where(x => x is Garnish)
			.Cast<Garnish>()
			.Select(x => x.garnishData)
			.ToList();

		return garnishes;
	}
}
