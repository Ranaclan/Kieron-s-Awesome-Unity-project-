using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class calculator : MonoBehaviour
{
    //ui
    private CanvasGroup alpha;
    //calculator
    private GameObject calculatorObject;
    private TMP_Text messages;
    //input field
    private TMP_InputField calculatorText;
    //calculations
    private List<string> first = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "-", "(" };
    private List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
    private List<string> calculations = new List<string> { "+", "-", "*", "/", "//", "^", "sqrt", "(", ")", "tan", "cos", "sin" };
    private List<string> brackets = new List<string> {};
    private string bracket;
    private List<string> calcNums = new List<string> {""};
    private int calcNumCount;
    private List<string> originalResult = new List<string> { "" };
    private string result;

    void Start()
    {
        //ui
        alpha = gameObject.GetComponent<CanvasGroup>();
        alpha.alpha = 0;
        //calculator
        calculatorObject = transform.GetChild(0).GetChild(0).gameObject;
        calculatorText = calculatorObject.GetComponent<TMP_InputField>();
        //messages
        messages = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    void Update()
    {
        UI();
        Calculator();
    }

    void UI()
    {
        //field type


        //opening
        if (Input.GetKeyDown("tab"))
        {
            if (alpha.alpha == 0)
            {
                //activate calculator
                alpha.alpha = 1;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (alpha.alpha == 1)
            {
                //deactivate calculator
                alpha.alpha = 0;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void cleanup(string text)
    {
        if(calculatorText.text.Length == 1)
        {
            //first character restrictions
            if(!first.Contains(calculatorText.text[0].ToString()))
            {
                calculatorText.text = calculatorText.text.Remove(0);
            }
        }
        if (calculatorText.text.Length > 0)
        {
            //space removal
            if (calculatorText.text[calculatorText.text.Length - 1] == ' ')
            {
                calculatorText.text = calculatorText.text.Remove(calculatorText.text.Length - 1);
            }

            //negative cleanups
            if(calculatorText.text[calculatorText.text.Length - 1] == '-')
            {
                if (calculatorText.text.Length > 1)
                {
                    //double negatives
                    if (calculatorText.text[calculatorText.text.Length - 2] == '-')
                    {
                        //remove two subtracts and replace with plus
                        calculatorText.text = calculatorText.text.Remove(calculatorText.text.Length - 1);
                        calculatorText.text = calculatorText.text.Remove(calculatorText.text.Length - 1);
                        if (!calculations.Contains(calculatorText.text[calculatorText.text.Length - 1].ToString()))
                        {
                            //if previous character is plus, do not add another plus
                            calculatorText.text = calculatorText.text.Insert(calculatorText.text.Length, "+");
                            calculatorText.caretPosition = calculatorText.text.Length;
                        }
                    }

                    //addition of negative
                    if(calculatorText.text[calculatorText.text.Length - 2] == '+')
                    {
                        //remove plus before subtract
                        calculatorText.text = calculatorText.text.Remove(calculatorText.text.Length - 2);
                        calculatorText.text = calculatorText.text.Insert(calculatorText.text.Length, "-");
                        calculatorText.caretPosition = calculatorText.text.Length;
                    }
                }
            }
        }
    }

    void Calculator()
    {
        if(Input.GetKeyDown("return"))
        {
            //part 1: get characters:
            //put all characters into calcNums list
            calcNums = new List<string> { "" };
            calcNumCount = 0;
            //check each character to put into list
            calcNums = makeList(calculatorText.text, 0, calculatorText.text.Length);

            //part 2: execute:
            printList(calcNums);
            result = calculate(calcNums);
            Debug.Log("result: " + result);
        }
    }

    string calculate(List<string> list)
    {
        //fix brackets pls
        //do calculations in list - brackets, indices, division, multiplication, addition, subtraction
        //brackets
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "(")
            {
                //removes opening bracket
                list.RemoveAt(i);
                //finds closing bracket
                for (int j = i; j < list.Count; j++)
                {
                    if (list[j] == ")")
                    {
                        //removes closing bracket
                        list.RemoveAt(j);
                        //gathers contents into new list while removing
                        brackets = new List<string> { };
                        for (int k = 0; k < j - i; k++)
                        {
                            brackets.Add(list[i]);
                            list.RemoveAt(i);
                        }
                        //calculate contents and put into list
                        list.Insert(i, calculate(brackets));
                    }
                }
            }
        }

        //indices
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "^")
            {
                if (i != 0 && i != list.Count - 1)
                {
                    if (betweenNumbers(list, i))
                    {
                        //removes two entries and replaces last with sum
                        list[i] = Mathf.Pow(float.Parse(list[i - 1]), float.Parse(list[i + 1])).ToString();
                        list.RemoveAt(i - 1);
                        list.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        messages.text = "Syntax Error";
                    }
                }
                else
                {
                    messages.text = "Syntax Error";
                }
            }
        }

        //division
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "/")
            {
                if (i != 0 && i != list.Count - 1)
                {
                    if (betweenNumbers(list, i))
                    {
                        //removes two entries and replaces last with sum
                        list[i] = (float.Parse(list[i - 1]) / float.Parse(list[i + 1])).ToString();
                        list.RemoveAt(i - 1);
                        list.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        messages.text = "Syntax Error";
                    }
                }
                else
                {
                    messages.text = "Syntax Error";
                }
            }
        }

        //multiplication
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "*")
            {
                if (i != 0 && i != list.Count - 1)
                {
                    if (betweenNumbers(list, i))
                    {
                        //removes two entries and replaces last with sum
                        list[i] = (float.Parse(list[i - 1]) * float.Parse(list[i + 1])).ToString();
                        list.RemoveAt(i - 1);
                        list.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        messages.text = "Syntax Error";
                    }
                }
                else
                {
                    messages.text = "Syntax Error";
                }
            }
        }

        //addition
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "+")
            {
                if (i != 0 && i != list.Count - 1)
                {
                    if (betweenNumbers(list, i))
                    {
                        //removes two entries and replaces last with sum
                        list[i] = (float.Parse(list[i - 1]) + float.Parse(list[i + 1])).ToString();
                        list.RemoveAt(i - 1);
                        list.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        messages.text = "Syntax Error";
                    }
                }
                else
                {
                    messages.text = "Syntax Error";
                }
            }
        }

        //subtraction
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "-")
            {
                if (i != 0 && i != list.Count - 1)
                {
                    if (betweenNumbers(list, i))
                    {
                        //removes two entries and replaces last with sum
                        list[i] = (float.Parse(list[i - 1]) - float.Parse(list[i + 1])).ToString();
                        list.RemoveAt(i - 1);
                        list.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        messages.text = "Syntax Error";
                    }
                }
                else
                {
                    messages.text = "Syntax Error";
                }
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

    void printList(List<string> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i]);
        }
    }

    List<string> makeList(string text, int start, int end)
    {
        int count = 0;
        List<char> list = new List<char> { };
        list.AddRange(text);
        List<string> newList = new List<string> { "" };

        for (int i = start; i < end; i++)
        {
            if (numbers.Contains((list[i]).ToString()))
            {
                //if character is number:
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

            if (calculations.Contains(list[i].ToString()))
            {
                //if character is calculation operator, add to new entry
                newList.Add((list[i].ToString()));
                count += 1;
            }
        }

        return newList;
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
        if(type == 2)
        {
            //returns text after index
            for(int i = index + 1; i < text.Length; i++)
            {
                localText += text[i];
            }
        }

        return localText;
    }
}
