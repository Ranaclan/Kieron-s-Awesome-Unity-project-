using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class calculator : MonoBehaviour
{
    //lists
    private List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
    private List<string> calculations = new List<string> { "+", "-", "*", "/", "//", "^", "(", ")", "T", "C", "S", "t", "c", "s" };
    private List<string> brackets = new List<string> { };
    private int openBrackets;

    List<string> makeList(string text, int start, int end)
    {
        //turn text into list of characters
        List<char> list = new List<char> { };
        list.AddRange(text);
        //create new list to collect contents of text correctly
        int count = 0;
        List<string> newList = new List<string> { "" };

        //search through text
        for (int i = start; i < end; i++)
        {
            //if character is number:
            if (numbers.Contains((list[i]).ToString()))
            {
                if (i > start + 1)
                {
                    if (numbers.Contains(list[i - 1].ToString()) || (list[i - 1] == '-' && calculations.Contains(list[i - 2].ToString())))
                    {
                        //if previous character is number, or previous character is subtract and previous previous character is another operator, 
                        //add new number to latest character as negative number
                        newList[count] += list[i];
                    }
                    else
                    {
                        //if not, add numbers to new entry
                        newList.Add(list[i].ToString());
                        count += 1;
                    }
                }
                else if (i == start + 1)
                {
                    if (numbers.Contains(list[start].ToString()) || list[start] == '-')
                    {
                        //if first character is number, or first character is subtract, add new number to latest character as negative number
                        newList[count] += list[i];
                    }
                    else
                    {
                        //if not, add numbers to new entry
                        newList.Add(list[i].ToString());
                        count += 1;
                    }
                }
                else
                {
                    //if not, add numbers to new entry
                    newList.Add(list[i].ToString());
                    count += 1;
                }
            }

            //if character is calculation operator:
            if (calculations.Contains(list[i].ToString()))
            {
                if (list[i] == '/' && list[i - 1] == '/')
                {
                    //if charcter is divide and previous character was also divide, add divide to last entry to form modulus
                    newList[count] += list[i];
                }
                else
                {
                    //if not, add character to new entry#
                    newList.Add((list[i].ToString()));
                    count += 1;
                }
            }

            //if character is gravitational constant:
            if (list[i] == 'G')
            {
                newList.Add("50");
                count += 1;
            }
            //if character is previous result:
            if (list[i] == 'A')
            {
                newList.Add(result);
                count += 1;
            }
        }
        return newList;
    }
}


