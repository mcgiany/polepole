namespace NakkaResults.Models;

public class RobinPoints
{
    public int FirstNonProgressPoints { get; set; }

    public int SecondNonProgressPoints { get; set; }

    public int OtherNonProgressPoints { get; set; }

    public RobinPoints(int firstNonProgressPoints, int secondNonProgressPoints, int otherNonProgressPoints)
    {
        FirstNonProgressPoints = firstNonProgressPoints;
        SecondNonProgressPoints = secondNonProgressPoints;
        OtherNonProgressPoints = otherNonProgressPoints;
    }
}
