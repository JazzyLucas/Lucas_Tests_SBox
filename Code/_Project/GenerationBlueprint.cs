using System;

public class GenerationBlueprint
{
	public int Seed { get; set; }
	public int NumberOfIterations { get; set; }
	public int NumberOfCheckpoints { get; set; }
	public int DistanceBetweenIterations { get; set; }
	public List<GameObject> ObjectsToUse { get; }
	public int MapWidth { get; set; }
	public int MapHeight { get; set; }
	public int MaxObjectSize { get; set; }
	public int MinObjectSize { get; set; }
	public bool DoRandomRotation { get; set; }
}
