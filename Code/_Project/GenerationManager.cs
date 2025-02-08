using System;
using System.Diagnostics;
using System.Numerics;

public sealed class GenerationManager : Component
{
	private static List<GameObject> objectPool;
	
	protected override void OnUpdate()
	{
		
	}

	private void Clear()
	{
		objectPool ??= new();
            
		// Clear previous generation
		foreach (var go in objectPool)
		{
			go.Transform.Parent.Clear();
		}
		objectPool.Clear();
	}
	
        private void Generate(GenerationBlueprint blueprint)
        {
	        Clear();
            
            // Initialization
            Random.InitState(blueprint.Seed);
            Vector3 startPosition = Vector3.Zero;
            int checkPointSpacing = blueprint.NumberOfIterations / blueprint.NumberOfCheckpoints;
            float distance = 0;
            List<int> objectsToUse = new();
            for (int i = 0; i < blueprint.ObjectsToUse.Count; i++)
            {
                if (blueprint.ObjectsToUse[i])
                    objectsToUse.Add(i);
            }

            // Generating the object position and the position's data
            for (int j = 0; j < blueprint.NumberOfCheckpoints; j++)
            {
                // TODO: Checkpoint spawning
                distance += 10f;
                for (int i = 0; i < checkPointSpacing; i++)
                {
                    int randomObjectIndex = Random.Range(0, objectsToUse.Count);
                    float width = Random.Range(-blueprint.MapWidth, blueprint.MapWidth);
                    float height = Random.Range(-blueprint.MapHeight, blueprint.MapHeight);
                    Vector3 position = new(startPosition.x + width, startPosition.y + height, startPosition.z + distance);
                    
                    // TODO: Use addressables instead of prefabs?
                    GameObject temp = Instantiate(PRI.generationConfigurationSO.generationObjects[randomObjectIndex], position, Quaternion.Identity);
                    GenerationObject generatedObject = temp.GetComponent<GenerationObject>();
                    objectPool.Add(temp);
                    
                    float randomSize = 1f;
                    
                    if (generatedObject.equalScaling)
                    {
                        float randomNum = Random.Range(blueprint.MinObjectSize, blueprint.MaxObjectSize);
                        temp.transform.localScale = new(randomNum, randomNum, randomNum);
                        randomSize = randomNum;
                    }
                    else
                    {
                        Vector3 localScale = new(
                            Random.Range(blueprint.MinObjectSize, blueprint.MaxObjectSize),
                            Random.Range(blueprint.MinObjectSize, blueprint.MaxObjectSize),
                            Random.Range(blueprint.MinObjectSize, blueprint.MaxObjectSize));
                        temp.transform.localScale = localScale;
                        randomSize = (localScale.x + localScale.y + localScale.z) / 3;
                    }
                    if (blueprint.DoRandomRotation)
                    {
                        temp.transform.rotation = Quaternion.Euler(new(Random.Range(0f,360f), Random.Range(0f,360f),Random.Range(0f,360f)));
                    }
                    generatedObject.ChangeColor(new(Random.Range(0f,1), Random.Range(0f,1),Random.Range(0f,1), 1f));
                    distance += randomSize + blueprint.DistanceBetweenIterations;
                }
            }
            
            // Now parent all of the objects to a single object in order to organize things
            GameObject checkIfNull = GameObject.Find("----- GenerationObjects");
            Transform parentT = checkIfNull == null ? new GameObject("----- GenerationObjects").transform : checkIfNull.transform;
            foreach (var go in objectPool)
            {
                go.transform.parent = parentT;
            }
        }
	
	private string MakeLogMessage(GenerationBlueprint bp)
	{
		string message = "";
		message += "\n Seed: " + bp.Seed;
		message += "\n Objects: ";
		message = bp.ObjectsToUse.Aggregate( message, ( current, objectToUse ) => current + (objectToUse + " ") );
		message += "\n Iterations: " + bp.NumberOfIterations;
		message += ", Checkpoints: " + bp.NumberOfCheckpoints;
		message += ", DistanceBetween: " + bp.DistanceBetweenIterations;
		message += ", Width: " + bp.MapWidth;
		message += ", Height: " + bp.MapHeight;
		message += ", MaxObjectSize: " + bp.MaxObjectSize;
		message += ", MinObjectSize: " + bp.MinObjectSize;
		message += ", DoRandomRot: " + bp.DoRandomRotation;
		return message;
	}
}
