using UnityEngine;

public class FoodTest : MonoBehaviour
{
    public FoodSpawner spawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner.StartSpawning();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
