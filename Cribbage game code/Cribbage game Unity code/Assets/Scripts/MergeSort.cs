using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeSort : MonoBehaviour
{
    public void SortArray(List<string> array, int left, int right)
    {
        if (left < right)
        {
            //Find the midpoint of the array
            int middle = left + (right - left) / 2;
            //Split the array in two until each has one element using recursion
            SortArray(array, left, middle);
            SortArray(array, middle + 1, right);
            MergeArray(array, left, middle, right);
        }
    }

    public void MergeArray(List<string> array, int left, int middle, int right)
    {
        //Define temporary arrays to hold values when merging
        var leftArrayLength = middle - left + 1;
        var rightArrayLength = right - middle;
        var leftTempArray = new string[leftArrayLength];
        var rightTempArray = new string[rightArrayLength];

        int leftValue;
        int rightValue;
        int counter1, counter2, pointer;

        //Copy data into temporary arrays
        for (counter1 = 0; counter1 < leftArrayLength; ++counter1)
            leftTempArray[counter1] = array[left + counter1];

        for (counter2 = 0; counter2 < rightArrayLength; ++counter2)
            rightTempArray[counter2] = array[middle + 1 + counter2];

        counter1 = 0;
        counter2 = 0;
        pointer = left;

        while (counter1 < leftArrayLength && counter2 < rightArrayLength)
        {
            //Get the number value from the string element
            leftValue = Convert.ToInt32(leftTempArray[counter1][(leftTempArray[counter1].LastIndexOf(' ') + 1)..]);
            rightValue = Convert.ToInt32(rightTempArray[counter2][(rightTempArray[counter2].LastIndexOf(' ') + 1)..]);

            //Traverse and compare elements from left and right array, if left is bigger than or equal to right then swap
            if (leftValue >= rightValue)
            {
                array[pointer++] = leftTempArray[counter1++];
            }
            else
            {
                array[pointer++] = rightTempArray[counter2++];
            }
        }

        //Copy remaining elements from left and right array into merged array
        while (counter1 < leftArrayLength)
        {
            array[pointer++] = leftTempArray[counter1++];
        }

        while (counter2 < rightArrayLength)
        {
            array[pointer++] = rightTempArray[counter2++];
        }
    }
}