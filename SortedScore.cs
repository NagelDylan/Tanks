// Author: Dylan Nagel
// File Name: SortedScores.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a sorted score, taking in and sorting data

using System;
using System.Collections.Generic;

namespace NagelD_PASS3
{
    public class SortedScore
    {
        //creates a variable to store all data
        List<float> allData;

        //pre: none
        //post: none
        //description: creates an instance of a sorted score object
        public SortedScore()
        {
            //sets up the list of all data
            allData = new List<float>();
        }

        //pre: a valid float representing the data being added
        //post: none
        //description: adds a piece of data to the all data list and sorts data
        public void AddData(float data)
        {
            //adds the new piece of data to list of data
            allData.Add(data);

            //sorts the data
            InsertionSort();
        }

        //pre: none
        //post: none
        //description: uses insertion sort to sort the data
        public void InsertionSort()
        {
            //create a float temporary variable
            float temp;

            //create an int x
            int x;

            //loop through all data starting at 1
            for (int i = 1; i < allData.Count; i++)
            {
                //Store the next element to be inserted
                temp = allData[i];

                //Shift all "sorted" elements that are greater than the new value to the right
                for (x = i; x > 0; x--)
                {
                    //the sorted value is greater than the insertion value
                    if (allData[x - 1] > temp)
                    {
                        //set data at index x to atat at index x - 1
                        allData[x] = allData[x - 1];
                    }
                    else
                    {
                        //break from the for loop
                        break;
                    }
                }

                //set all data at index x to the temporary variable
                allData[x] = temp;
            }
        }

        //pre: none
        //post: a valid float representing the median value
        //description: returns the median value from the list
        public float GetMedian()
        {
            //checks if there is no data in the data list
            if(allData.Count == 0)
            {
                //returns zero
                return 0;
            }

            //creates a variable and stores the mid point
            int midPoint = allData.Count / 2; ;

            //checks if all data is an even number
            if (allData.Count % 2 == 0)
            {
                //returns the mid point value
                return (float)Math.Round((allData[midPoint] + allData[midPoint - 1]) * 0.5, 2);
            }

            //sets the midpoint
            midPoint = allData.Count / 2;

            //returns the midpiont value
            return (float)Math.Round(allData[midPoint], 2);
        }

        //pre: none
        //post: a float representing the average
        //description: gets the average of the data
        public float GetAverage()
        {
            //checks if there is no data
            if (allData.Count == 0)
            {
                //returns zero
                return 0;
            }

            //creates a float to hold total value
            float total = 0;

            //loop through all the data
            for(int i = 0; i < allData.Count; i++)
            {
                //add data at index i to total
                total += allData[i];
            }

            //return the average
            return (float)Math.Round(total / allData.Count, 2);
        }

        //pre: none
        //post: a string representing all the data seperated by commas
        //description: returns all the data as a string
        public string ReturnAll()
        {
            //creates variable to store all data
            string allInfo = "";

            //loops through all the data
            for (int i = 0; i < allData.Count; i++)
            {
                //adds the dtat to the total string
                allInfo += allData[i] + ",";
            }

            //returns all info
            return allInfo;
        }

        //pre: none
        //post: none
        //description: resets the data
        public void ResetData()
        {
            //clears the all data list
            allData.Clear();
        }
    }
}
