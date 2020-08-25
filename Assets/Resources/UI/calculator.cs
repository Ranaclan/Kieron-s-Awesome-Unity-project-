using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class calculator : MonoBehaviour
{
    //ui
    private TMP_InputField calculatorText;
    private GameObject calculatorObject;
    private TMP_Text messages;
    //cleanup
    private List<string> first = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "-", "(", "T", "C", "S", "t", "c", "s", " " };
    private List<char> noAdjacent = new List<char> { '^', '*', '+' };
    private float textLength;
    private bool removeLast;
    //lists
    //private List<string> letters = new List<string> { "a", "0", "b", "0", "c", "0", "d", "0", "e", "0", "f", "0", "g", "0", "h", "0", "i", "0", "j", "0", "k", "0", "l", "0", "m", "0", "n", "0", "o", "0", "p", "0", "q", "0", "r", "0", "s", "0", "t", "0", "u", "0", "v", "0", "w", "0", "x", "0", "y", "0", "z", "0" };
    private List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
    private List<string> calculations = new List<string> { "+", "-", "*", "/", "//", "^", "(", ")", "T", "C", "S", "t", "c", "s" };
    private List<char> trig = new List<char> { 'T', 'C', 'S', 't', 'c', 's' };
    //brackets
    //private List<string> brackets = new List<string> {};
    private List<string> brackets = new List<string> { };
    private int openBrackets;
    //calculations
    private List<string> calcNums = new List<string> { "" };
    private int calcNumCount;
    private string result;

    private float frameCount;

    void Start()
    {
        //calculator
        calculatorObject = transform.GetChild(0).gameObject;
        calculatorText = calculatorObject.GetComponent<TMP_InputField>();
        //messages
        messages = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    void Update()
    {
        UI();
        Calculator();
    }

    void UI()
    {
        //field type


    }

    public void cleanup(string text)
    {
        //serach through text
        for (int i = 0; i < text.Length; i++)
        {
            //space removal
            if (text[i] == ' ')
            {
                remove(i);
            }

            if (i < text.Length - 1)
            {
                //subtract
                if (text[i] == '-')
                {
                    //double subtract
                    if (text[i + 1] == '-')
                    {
                        remove(i + 1);
                        remove(i);
                        calculatorText.text = calculatorText.text.Insert(i, "+");
                        calculatorText.caretPosition = i + 1;
                    }

                    //subtract plus
                    if (text[i + 1] == '+')
                    {
                        remove(i + 1);
                        calculatorText.caretPosition = i + 1;
                    }
                }

                //plus
                if (text[i] == '+')
                {
                    //plus subtract
                    if (text[i + 1] == '-')
                    {
                        remove(i);
                    }
                }

                //division
                if (text[i] == '/')
                {
                    //triple division or no adjacent
                    if ((text[i - 1] == '/' && text[i + 1] == '/') || noAdjacent.Contains(text[i + 1]))
                    {
                        remove(i + 1);
                    }
                }

                //no adjacent operators
                if (noAdjacent.Contains(text[i]) && noAdjacent.Contains(text[i + 1]))
                {
                    remove(i + 1);
                }
            }

            if (text.Length > 0)
            {
                //first character restrictions
                if (i == 0)
                {
                    //first character must be in 'first' list
                    if (!first.Contains(text[0].ToString()))
                    {
                        remove(0);
                    }
                }
            }
        }
    }

    private void remove(int i)
    {
        calculatorText.text = calculatorText.text.Remove(i, 1);
    }

    void Calculator()
    {
        if (Input.GetKeyDown("return"))
        {
            //part 1: do final cleanup of input
            FinalCleanup();

            //part 2: put all characters into calcNums list and check through list to perform calculations
            calcNums = new List<string> { "" };
            calcNumCount = 0;
            calcNums = makeList(calculatorText.text, 0, calculatorText.text.Length);

            //part 3: final executution:
            result = calculate(calcNums);
            //result = doCalculate(calcNums);
            Debug.Log("result: " + result);
            messages.text = "Result: " + (Mathf.Round(float.Parse(result) * 100) / 100);
        }
    }

    void FinalCleanup()
    {
    }

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
                        //if previous character is number, or previous character is subtract and previous previous character is another operator, add new number to latest character as negative number
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
                    //if not, add character to new entry
                    newList.Add((list[i].ToString()));
                    count += 1;
                }
            }
        }

        return newList;
    }

    string doCalculate(List<string> list)
    {
        //search through characters
        for (int i = 0; i < list.Count; i++)
        {
            //tangent
            if (doTrig("T"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Tan(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //cosine
            if (doTrig("C"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Cos(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //sine
            if (doTrig("S"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Sin(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //inverse tangent
            if (doTrig("t"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Atan(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //inverse cosine
            if (doTrig("c"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Acos(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //inverse sine
            if (doTrig("s"))
            {
                //replaces number with tan of it
                list[i] = Mathf.Asin(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                //next loop
                i--;
                continue;
            }

            //indices
            if (doCalculation("^"))
            {
                //removes two entries and replaces last with product
                list[i] = Mathf.Pow(float.Parse(list[i - 1]), float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            //modulo
            if (doCalculation("//"))
            {
                //removes two entries and replaces last modulus
                list[i] = (float.Parse(list[i - 1]) % float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            //division
            if (doCalculation("/"))
            {
                //removes two entries and replaces last with product
                list[i] = (float.Parse(list[i - 1]) / float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            //multiplication
            if (doCalculation("*"))
            {
                //removes two entries and replaces last with product
                list[i] = (float.Parse(list[i - 1]) * float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            //addition
            if (doCalculation("+"))
            {
                //removes two entries and replaces last with sum
                list[i] = (float.Parse(list[i - 1]) + float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            Debug.Log("a");

            //subtraction
            if (doCalculation("-"))
            {
                //removes two entries and replaces last with sum
                list[i] = (float.Parse(list[i - 1]) - float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                //next loop
                i -= 2;
                continue;
            }

            bool doTrig(string calculation)
            {
                //check calculation
                if (list[i] == calculation && i != list.Count - 1)
                {
                    string after = list[i + 1];
                    if (numbers.Contains(after[after.Length - 1].ToString()))
                    {
                        //remove trig function and return true
                        list.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            bool doCalculation(string calculation)
            {
                Debug.Log(list[i]);
                //check calculation
                if (list[i] == calculation)
                {
                    if (i != 0 && i != list.Count - 1)
                    {
                        //check numbers on either side of operator
                        if (betweenNumbers(list, i))
                        {
                            //return true
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        //create result
        result = "";
        for (int i = 0; i < list.Count; i++)
        {
            result += list[i];
        }

        return result;
    }

    string calculate(List<string> list)
    {
        //do calculations in list
        int j;
        openBrackets = 0;
        brackets = new List<string> { };

        //printList(list);

        //basic calculations: brackets, trigonometry, indices, modulo, division, multiplication, addition, subtraction

        //brackets
        /*
        for (int i = 0; i < list.Count; i++)
        {
            if (openBrackets == 0)
            {
                if (list[i] == "(")
                {
                    //initial open bracket
                    brackets = new List<string> { };
                    openBrackets = 1;
                    bracketDo = true;
                    list.RemoveAt(i);
                }
            }
            else
            {
                Debug.Log(openBrackets);
                //new bracket  opened
                if (list[i] == ")")
                {
                    //close one bracket
                    openBrackets -= 1;
                    list.RemoveAt(i);

                    if(openBrackets == 0)
                    {
                        Debug.Log("/ " + calculate(brackets));
                    }
                }
                else if (list[i] == "(")
                {
                    //open another bracket
                    openBrackets += 1;
                    list.RemoveAt(i);
                }
                else
                {
                    //add brackets contents to brackets list
                    brackets.Add(list[i]);
                    list.RemoveAt(i);
                }
            }
        }*/

        //brackets
        for (int i = 0; i < list.Count; i++)
        {
            //brackets
            if (list[i] == "(" || list[i] == ")")
            {
                if (list[i] == "(")
                {
                    //open bracket
                    if (openBrackets == 0)
                    {
                        //if brackets only now open
                        list.RemoveAt(i);
                    }

                    openBrackets++;
                    i--;
                    continue;
                }

                if (list[i] == ")")
                {
                    //close bracket
                    openBrackets--;

                    //if brackets now closed
                    if (openBrackets == 0)
                    {
                        //list[i] = calculate(brackets);
                        list[i] = "9";
                    }

                    i--;
                }
            }
            else
            {
                //brackets content
                if (openBrackets > 0)
                {
                    brackets.Add(list[i]);
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        /*
        //brackets
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "(")
            {
                openBrackets += 1;
                brackets.Add(new List<string> { i.ToString() });
            }

            if (list[i] == ")")
            {
                openBrackets -= 1;
            }

            if(openBrackets > 0)
            {
                brackets[openBrackets - 1].Add(list[i]);
                list.RemoveAt(i);
                i--;
            }
        }
        
        //execute brackets
        if(openBrackets > 0)
        {

        }

        for(int i = 0; i < brackets.Count; i++)
        {
            printList(brackets[i]);
        }
        */

        //tangent
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("T", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Tan(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //cosine
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("C", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Cos(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //sine
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("S", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Sin(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //inverse tangent
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("t", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Atan(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //inverse cosine
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("c", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Acos(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //inverse sine
        for (int i = 0; i < list.Count; i++)
        {
            if (doTrig("s", i))
            {
                //replace number with tan of it
                list[i] = Mathf.Asin(float.Parse(list[i]) * Mathf.Deg2Rad).ToString();
                i--;
            }
        }

        //indices
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("^", i))
            {
                //removes two entries and replaces last with product
                list[i] = Mathf.Pow(float.Parse(list[i - 1]), float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        //modulo
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("//", i))
            {
                //removes two entries and replaces last with modulus
                list[i] = (float.Parse(list[i - 1]) % float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        //division
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("/", i))
            {
                //removes two entries and replaces last with product
                list[i] = (float.Parse(list[i - 1]) / float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        //multiplication
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("*", i))
            {
                //removes two entries and replaces last with product
                list[i] = (float.Parse(list[i - 1]) * float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        //addition
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("+", i))
            {
                //removes two entries and replaces last with sum
                list[i] = (float.Parse(list[i - 1]) + float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        //subtraction
        for (int i = 0; i < list.Count; i++)
        {
            if (doCalculation("-", i))
            {
                //removes two entries and replaces last with sum
                list[i] = (float.Parse(list[i - 1]) - float.Parse(list[i + 1])).ToString();
                list.RemoveAt(i - 1);
                list.RemoveAt(i);
                i -= 2;
            }
        }

        bool doTrig(string calculation, int i)
        {
            //check for calculation and place
            if (list[i] == calculation && i != list.Count - 1)
            {
                //check for number afterwards
                string after = list[i + 1];
                if (numbers.Contains(after[after.Length - 1].ToString()))
                {
                    //remove trig function and return number's index
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        bool doCalculation(string calculation, int i)
        {
            //check for calculation and place
            if (list[i] == calculation && i != 0 && i != list.Count - 1)
            {
                //check numbers on either side of operator
                if (betweenNumbers(list, i))
                {
                    return true;
                }
            }

            return false;
        }

        //create result
        result = "";
        for (int i = 0; i < list.Count; i++)
        {
            result += list[i];
        }

        return result;
    }



    bool betweenNumbers(List<string> list, int i)
    {
        //check if calculation operator is between two numbers
        //take entries in list on either side of operator
        string before = list[i - 1];
        string after = list[i + 1];
        //check if entries include numbers as last character of string
        if (numbers.Contains(before[before.Length - 1].ToString()) && numbers.Contains(after[after.Length - 1].ToString()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void printList(List<string> list)
    {
        string output = "";
        for (int i = 0; i < list.Count; i++)
        {
            output += list[i];
        }

        Debug.Log(output);
    }

    public string splitText(string text, int index, int type)
    {
        string localText = "";

        if (type == 1)
        {
            //returns text before index
            for (int i = 0; i < index; i++)
            {
                localText += text[i];
            }
        }
        if (type == 2)
        {
            //returns text after index
            for (int i = index + 1; i < text.Length; i++)
            {
                localText += text[i];
            }
        }

        return localText;
    }
}


