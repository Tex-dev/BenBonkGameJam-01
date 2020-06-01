using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static int[] GenerateListRandomNUmber(int min, int max, int nbValue)
    {
        List<int> numbers = new List<int> (max-min);
        for (int i = min; i < max; i++)
        {
            numbers.Add(i);
        }
        
        int[] randomNumbers = new int[nbValue];

        for (int i = 0; i < randomNumbers.Length; i++)
        {
            var thisNumber = Random.Range(0, numbers.Count);
            randomNumbers[i] = numbers[thisNumber];
            numbers.RemoveAt(thisNumber);
        }

        return randomNumbers;
    }
}
