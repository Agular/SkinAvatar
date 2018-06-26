using Avatar;
using System.Collections.Generic;

public class Filter
{
    protected List<Body> BodySamples;
    protected int MaxSamples;

    public Filter(int maxSamples)
    {
        MaxSamples = maxSamples;
        BodySamples = new List<Body>();
    }

    public virtual Body GetFilteredData(Body body)
    {
        return null;
    }

    public void Reset()
    {
        BodySamples.Clear();
    }

    protected void AddToSamples(Body body)
    {

        BodySamples.Insert(0, body);

        while (BodySamples.Count > MaxSamples)
        {
            BodySamples.RemoveAt(BodySamples.Count - 1);
        }
    }
}
