
public class FisherYatesRandom
{
    private int[] randomIndices = null;
    private int randomIndex = 0;
    private int prevValue = -1;

    public int Next(int len)
    {
        if (len <= 1)
            return 0;

        if (randomIndices == null || randomIndices.Length != len)
        {
            randomIndices = new int[len];
            for (int i = 0; i < randomIndices.Length; i++)
                randomIndices[i] = i;
        }

        if (randomIndex == 0)
        {
            int count = 0;
            do
            {
                for (int i = 0; i < len - 1; i++)
                {
                    int j = UnityEngine.Random.Range(i, len);
                    if (j != i)
                    {
                        int tmp = randomIndices[i];
                        randomIndices[i] = randomIndices[j];
                        randomIndices[j] = tmp;
                    }
                }
            } while (prevValue == randomIndices[0] && ++count < 10); // Make sure the new first element is different from the last one we played
        }

        int value = randomIndices[randomIndex];
        if (++randomIndex >= randomIndices.Length)
            randomIndex = 0;

        prevValue = value;
        return value;
    }
}
