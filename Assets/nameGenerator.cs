using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nameGenerator : MonoBehaviour
{
    private string word;
    private int length;
    private char[] consonant = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
    private char[] vowel = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
    private int letterType;
    private int consonants = 0;
    private int vowels = 0;

    public void Select(int seed)
    {
        Random.InitState(seed);
        //new word of length 3 to 10 letters
        word = "";
        length = Random.Range(3, 10);

        //loop through letters
        for (int i = 0; i <= length; i++)
        {
            if (consonants < 2)
            {
                if (vowels < 3)
                {
                    //if either consonant or vowels are allowed, randomly select one
                    letterType = Random.Range(0, 1);
                    Add(letterType);
                }
                else
                {
                    //only consonant allowed
                    Add(0);
                }
            }
            else
            {
                //only vowel allowed
                Add(1);
            }
        }

        char[] characters = word.ToCharArray();
        characters[0] = char.ToUpper(characters[0]);
        word = new string(characters);
        GameObject.Find("Player").GetComponent<player>().planetName = word;
    }

    void Add(int type)
    {
        int position;

        switch (type)
        {
            case 0:
                //consonant
                position = Random.Range(0, 19);
                word += consonant[position];
                consonants += 1;
                vowels = 0;
                break;
            case 1:
                //vowel
                position = Random.Range(0, 5);
                word += vowel[position];
                vowels += 1;
                consonants = 0;
                break;
        }
    }
}
