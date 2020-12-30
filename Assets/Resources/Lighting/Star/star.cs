using UnityEngine;

public class star : MonoBehaviour
{
    public void Stars(int starValue)
    {
        //stars:
        //1 - 6 = 1 star
        //6 - 9 = 2 stars
        //10 = 3 stars
        int stars;
        if(starValue < 7)
        {
            stars = 0;
        }
        else if(starValue < 9)
        {
            stars = 1;
        }
        else
        {
            stars = 2;
        }

        for (int i = 0; i < 3; i++)
        {
            Transform star = transform.GetChild(i);
            if (i > stars)
            {
                star.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                Material starMat = star.GetComponent<Renderer>().material;

                //hsv colour: s <= 70 <= v
                Vector3 starColour = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 0.4f), Random.Range(0.7f, 1f));
                float starMatIntensity = Random.Range(2f, 6.5f);
                starMat.EnableKeyword("_EMISSION");
                starMat.SetColor("_EmissionColor", Color.HSVToRGB(starColour.x, starColour.y, starColour.z) * starMatIntensity);

                star.position = new Vector3(-850, Random.Range(300, 600), Random.Range(-500, 500)); ;
                float starSize = Random.Range(50, 120);
                star.localScale = new Vector3(starSize, starSize, starSize);
            }
        }
    }
}
